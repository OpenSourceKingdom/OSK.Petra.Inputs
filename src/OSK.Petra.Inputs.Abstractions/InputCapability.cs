using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions;

public abstract class InputCapability<TInput> : IInputCapability
    where TInput : IInput
{
    #region IInputCapability

    public bool CanProces(IInput input)
        => input is TInput;

    public void Process(IUserInputContext context, IInput input)
    {
        if (input is not null && input is TInput typedInput)
        {
            Process(context, typedInput);
        }
    }

    #endregion

    #region Helpers

    protected abstract void Process(IUserInputContext context, TInput input);

    #endregion
}
