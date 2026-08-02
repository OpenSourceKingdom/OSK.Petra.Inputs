using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions;

public interface IInputCapability
{
    bool CanProces(IInput input);

    void Process(IUserInputContext context, IInput input);
}
