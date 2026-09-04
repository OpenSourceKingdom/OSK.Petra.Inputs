using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public class DevicePage(DeviceTopologyName topologyName, IEnumerable<IDeviceDescriptor> deviceDescriptors)
{
    public DeviceTopologyName TopologyName { get; } = topologyName;

    public IDeviceDescriptor? GenericDevice { get; } = deviceDescriptors?.FirstOrDefault(descriptor => descriptor.IsGeneric());

    public IReadOnlyList<IDeviceDescriptor> Devices { get; set; } = deviceDescriptors?.ToArray() ?? [];
}
