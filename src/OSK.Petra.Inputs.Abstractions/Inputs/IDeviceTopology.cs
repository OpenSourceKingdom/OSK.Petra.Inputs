using System.Collections.Generic;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public interface IDeviceTopology
{
    DeviceTopologyName Name { get; }

    IReadOnlyCollection<IInput> Inputs { get; }
}
