using Microsoft.Extensions.Options;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Power;

public class PowerCapability(IOptions<PowerCapabilityOptions> options) : InputCapability<IPowerInput>
{
    #region InputCapability Overrides

    protected override void Process(IDeviceInputContext context, IInputState state, IPowerInput input, TimeSpan deltaTime)
    {
        var details = state.GetOrCreateDetails<PowerDetails>();

        switch (state.Phase)
        {
            case InputPhase.End:
                if (!input.Settings.AllowReactivation || options.Value.ReactivationTime is null || details.TimeSinceLastActivation >= options.Value.ReactivationTime)
                {
                    state.Dispose();
                }
                break;
            default:
                var nextPhase = input.Power >= input.Settings.ActivationSensitivityThreshold
                    ? state.Duration >= options.Value.ActiveTimeThreshold ? InputPhase.Active : InputPhase.Start
                    : InputPhase.End;

                state.CombinePhase(nextPhase);

                var elapsedSeconds = details.TimeSinceLastActivation.TotalSeconds;
                details.Acceleration = input.Axis == details.Axis && elapsedSeconds > 0
                    ? (input.Power - details.Power) / elapsedSeconds
                    : 0;

                details.Power = input.Power;
                details.Axis = input.Axis;
                details.TimeSinceLastActivation = TimeSpan.Zero;
                details.ActivationCount = input.Settings.AllowReactivation
                    ? details.ActivationCount + 1
                    : 1;
                break;
        }

        details.TimeSinceLastActivation += deltaTime;
    }

    #endregion
}
