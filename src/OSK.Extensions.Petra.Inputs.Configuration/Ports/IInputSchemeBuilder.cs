using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IInputSchemeBuilder
{
    string Name { get; }

    IInputSchemeBuilder MakeDefault();

    IInputSchemeBuilder WithMap(DeviceIdentity deviceIdentity, long inputId, string actionName);
}
