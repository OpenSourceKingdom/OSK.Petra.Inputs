using Microsoft.Extensions.Options;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions;

public abstract class InputCapability<TInput, TOptions>(IOptions<TOptions> options) : InputCapability<TInput> 
    where TInput : IInput
    where TOptions: class
{
    #region InputCapability Overrides

    protected override void Process(IUserInputContext context, TInput input)
        => Process(context, input, options.Value);

    #endregion

    #region Helpers

    protected abstract void Process(IUserInputContext context, TInput input, TOptions options);

    #endregion
}
