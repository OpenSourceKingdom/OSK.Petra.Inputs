using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IDeviceMapBuilder
{
    IDeviceMapBuilder WithMap(IInput input, string actionName);
}
