using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Models;

public class DeviceCatalog
{
    public DeviceTopologyName TopologyName { get; set; }

    public IDeviceDescriptor? GenericDevice { get; set; }

    public IReadOnlyList<IDeviceDescriptor> KnownDevices { get; set; } = [];
}
