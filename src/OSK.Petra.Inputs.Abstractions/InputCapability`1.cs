using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions;

public abstract class InputCapability<TInputEvent, TSettings> : InputCapability<TInputEvent>
    where TInputEvent : IInputEvent
    where TSettings : IInputSettings, new()
{
    #region InputCapability Overrides

    protected override void Process(IUserInputContext context, IInputState state, TInputEvent inputEvent, TimeSpan deltaTime)
    {
        var settings = state.Input is IInput<TSettings> settingsInput
            ? settingsInput.Settings
            : new TSettings();

        Process(context, state, inputEvent, settings, deltaTime);
    }

    #endregion

    #region Helpers

    protected abstract void Process(IUserInputContext context, IInputState state, TInputEvent inputEvent, TSettings inputSettings, TimeSpan deltaTime);

    #endregion
}
