using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Models;

public class DevicePage(DeviceTopologyName topologyName, IEnumerable<IDeviceDescriptor> deviceDescriptors)
{
    public DeviceTopologyName TopologyName { get; } = topologyName;

    public IDeviceDescriptor? GenericDevice { get; } = deviceDescriptors?.FirstOrDefault(descriptor => descriptor.IsGeneric());

    public IReadOnlyList<IDeviceDescriptor> Devices { get; set; } = deviceDescriptors?.ToArray() ?? [];
}
