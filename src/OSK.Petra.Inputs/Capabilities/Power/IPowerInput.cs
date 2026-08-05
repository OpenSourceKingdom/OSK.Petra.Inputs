using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Petra.Inputs.Capabilities.Power;

public interface IPowerInput: IInput<PowerSettings>
{
    float Power { get; }

    PowerAxis Axis { get; }
}
