using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IInputSchemeBuilder
{
    string Name { get; }

    IInputSchemeBuilder MakeDefault();

    IInputSchemeBuilder WithMap(DeviceIdentity deviceIdentity, IInput input, string actionName);
}
