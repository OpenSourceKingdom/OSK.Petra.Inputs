using Microsoft.Extensions.Logging;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Ports;
using System;
using System.Linq;

namespace OSK.Petra.Inputs.Capabilities.Power;

/// <summary>
/// A capability that is able to process <see cref="PowerEvent"/>
/// </summary>
/// <param name="options">The options configuration for power input processing</param>
/// <param name="logger">A logger to log detail input processing information</param>
public partial class PowerCapability(ICapabilityOptions<PowerCapabilityOptions> options, ILogger<PowerCapability> logger) : InputCapability<PowerEvent, PowerSettings>
{
    #region InputCapability Overrides

    protected override void Process(IUserInputContext context, IInputState state, PowerEvent powerEvent, PowerSettings settings, TimeSpan deltaTime)
    {
        var details = state.GetOrCreateDetails<PowerDetails>();

        ProcessPowerInput(context, state, powerEvent, details, settings);
        if (!state.IsDisposed)
        {
            ProcessCombinationInputs(context, state, powerEvent, details, settings);
        }

        details.TimeSinceLastActivation += deltaTime;
    }

    #endregion

    #region Helpers

    private void ProcessCombinationInputs(IUserInputContext context, IInputState currentState, PowerEvent powerEvent, PowerDetails details, PowerSettings settings)
    {
        if (currentState.IsConsumed() || currentState is not DeviceInputState deviceInputState)
        {
            return;
        }

        DeviceInputIdentifier deviceInputIdentifier = new(deviceInputState.DeviceIdentifier.DeviceIdentity, deviceInputState.DeviceInput.Id);

        // Only check for combinations that are not currently triggered.
        foreach (var combinationInput in context.VirtualInputContext.GetInputs<IPowerCombinationInput>().Where(input => !context.VirtualInputContext.TryGetState(input, out _)))
        {
            if (!combinationInput.InputIdentifiers.Any(identifier => identifier.Matches(deviceInputIdentifier)))
            {
                continue;
            }

            var combinationInputStates = combinationInput.InputIdentifiers
                                                         .Select(identifier => context.TryGetInputState(identifier.DeviceIdentity, identifier.InputId, out var state) && !state.IsConsumed() ? state : null)
                                                         .Where(state => state is not null)
                                                         .OfType<DeviceInputState>()
                                                         .ToArray();

            // All inputs must be activated, and not consumed, in order to activate the combination
            if (combinationInputStates.Length != combinationInput.InputIdentifiers.Count)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    LogCombinationInputIgnored(logger,
                        $"[{string.Join(", ", combinationInput.InputIdentifiers.Select(identifier => $"Device: {identifier.DeviceIdentity} Input: {identifier.InputId}"))}]",
                        $"[{string.Join(", ", combinationInputStates.Select(state => $"Device: {state.DeviceIdentifier.DeviceIdentity} Input: {state.DeviceInput.Id}"))}]");
                }
                continue;
            }

            var combinationState = context.VirtualInputContext.GetOrCreateState(combinationInput, () => [PowerEvent.Full()]);
            LogCombinationInputTriggered(logger, $" Combined Inputs: [{string.Join(", ", combinationInputStates.Select(state => $"Device: {state.DeviceIdentifier.DeviceIdentity} Input: {state.DeviceInput.Id}"))}]");

            foreach (var combinationInputState in combinationInputStates)
            {
                combinationState.TryConsume(combinationInputState!);
                LogInputConsumed(logger, combinationInputState.DeviceIdentifier.DeviceIdentity, combinationInputState.DeviceInput.Id);
            }
        }
    }

    private void ProcessPowerInput(IUserInputContext context, IInputState state, PowerEvent powerEvent, PowerDetails details, PowerSettings settings)
    {
        switch (state.Phase)
        {
            case InputPhase.End:
                if (!settings.AllowReactivation || options.Value.ReactivationTime is null || details.TimeSinceLastActivation >= options.Value.ReactivationTime)
                {
                    state.Dispose();
                    LogPowerInputTerminated(logger);
                }
                break;
            default:

                // No need to track and monitor the input if it's a new event and below the power threshold required to initiate
                if (state.IsNewActivation && powerEvent.Power < settings.PowerSensitivityThreshold)
                {
                    state.Dispose();
                    LogPowerInputTerminated(logger);
                    return;
                }

                var nextPhase = powerEvent.Power >= settings.PowerSensitivityThreshold
                    ? state.Duration >= options.Value.ActiveTimeThreshold ? InputPhase.Active : InputPhase.Start
                    : InputPhase.End;

                state.CombinePhase(nextPhase);

                var elapsedSeconds = details.TimeSinceLastActivation.TotalSeconds;
                details.Acceleration = powerEvent.Axis == details.Axis && elapsedSeconds > 0
                    ? (powerEvent.Power - details.Power) / elapsedSeconds
                    : 0;

                details.Power = powerEvent.Power;
                details.Axis = powerEvent.Axis;
                details.TimeSinceLastActivation = TimeSpan.Zero;
                details.ActivationCount = settings.AllowReactivation
                    ? details.ActivationCount + 1
                    : 1;

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    LogInputEventProcessed(logger, state.Phase,
                        state.Input is IPowerCombinationInput combinationInput
                            ? $"combination [{string.Join(", ", combinationInput.InputIdentifiers.Select(identifier => $"Device: {identifier.DeviceIdentity} Input: {identifier.InputId}"))}]"
                            : $"single Device: {((DeviceInputState)state).DeviceIdentifier.DeviceIdentity} Input: {((DeviceInputState)state).DeviceInput.Id}");
                }

                break;
        }
    }

    #endregion

    #region Logging

    [LoggerMessage(eventId: 1, LogLevel.Debug, "A combination was triggered - {details}")]
    private static partial void LogCombinationInputTriggered(ILogger logger, string details);

    [LoggerMessage(eventId: 2, LogLevel.Debug, "A power input was consumed - device: {deviceIdentity} input id: {inputId}")]
    private static partial void LogInputConsumed(ILogger logger, DeviceIdentity deviceIdentity, long inputId);

    [LoggerMessage(eventId: 3, LogLevel.Debug, "A combination input could not be trigged - expected inputs: {expectedInputText} actual inputs: {actualInputText}")]
    private static partial void LogCombinationInputIgnored(ILogger logger, string expectedInputText, string actualInputText);

    [LoggerMessage(eventId: 4, LogLevel.Debug, "An input power event was processed. Input Phase: {inputPhase} for input {details}")]
    private static partial void LogInputEventProcessed(ILogger logger, InputPhase inputPhase, string details);

    [LoggerMessage(eventId: 5, LogLevel.Debug, "A power input has been terminated.")]
    private static partial void LogPowerInputTerminated(ILogger logger);

    #endregion
}
