using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions;

/// <summary>
/// A base class that provides a way to strongly type expected input events and input settings for the capability
/// </summary>
/// <typeparam name="TInputEvent">The type of event the capability is expected to process</typeparam>
/// <typeparam name="TSettings">The settings the input is expected to provide</typeparam>
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
