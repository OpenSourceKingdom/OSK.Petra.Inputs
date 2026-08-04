using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Abstractions;

public abstract class InputCapability<TInput> : IInputCapability
    where TInput : IInput
{
    #region IInputCapability

    public bool CanProces(IInput input)
        => input is TInput;

    public void Process(IDeviceInputContext context, IInputState state, TimeSpan deltaTime)
    {
        if (context is not null && state is not null && state.Input is TInput typedInput)
        {
            Process(context, state, typedInput, deltaTime);
        }
    }

    #endregion

    #region Helpers

    protected abstract void Process(IDeviceInputContext context, IInputState state, TInput input, TimeSpan deltaTime);

    #endregion
}
