using Microsoft.Extensions.Options;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Digital;

internal class DigitalInputCapability(IOptions<DigitalInputOptions> options) : InputCapability<IDigitalInput>
{
    #region InputCapability Overrides

    protected override void Process(IDeviceInputContext context, IInputState state, IDigitalInput input, TimeSpan deltaTime)
    {
        state.Phase = input.On
            ? state.Duration >= options.Value.ActiveTimeThreshold ? InputPhase.Active : InputPhase.Start 
            : InputPhase.End;


    }

    #endregion
}
