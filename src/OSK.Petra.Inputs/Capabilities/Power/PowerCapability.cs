using Microsoft.Extensions.Options;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Power;

public class PowerCapability(IOptions<PowerCapabilityOptions> options) : InputCapability<PowerEvent, PowerSettings>
{
    #region InputCapability Overrides

    protected override void Process(IDeviceInputContext context, IInputState state, PowerEvent powerEvent, PowerSettings settings, TimeSpan deltaTime)
    {
        var details = state.GetOrCreateDetails<PowerDetails>();

        switch (state.Phase)
        {
            case InputPhase.End:
                if (!settings.AllowReactivation || options.Value.ReactivationTime is null || details.TimeSinceLastActivation >= options.Value.ReactivationTime)
                {
                    state.Dispose();
                }
                break;
            default:
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
                break;
        }

        details.TimeSinceLastActivation += deltaTime;
    }

    #endregion
}
