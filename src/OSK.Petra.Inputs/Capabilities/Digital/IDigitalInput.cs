using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Capabilities.Digital;

public interface IDigitalInput: IInput
{
    bool On { get; }
}
