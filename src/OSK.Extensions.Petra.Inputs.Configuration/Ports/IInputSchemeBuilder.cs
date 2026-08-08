using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IInputSchemeBuilder
{
    IInputSchemeBuilder MakeDefault();

    IInputSchemeBuilder WithDevice(DeviceInputMap map);
}
