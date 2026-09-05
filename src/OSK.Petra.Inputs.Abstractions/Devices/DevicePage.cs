using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Represents an entire page of device information for a given <see cref="DeviceTopologyName"/> within a <see cref="DeviceCatalog"/>
/// </summary>
/// <param name="topologyName">The topology the page refers to</param>
/// <param name="deviceDescriptors">The devices the page contains</param>
public class DevicePage(DeviceTopologyName topologyName, IEnumerable<IDeviceDescriptor> deviceDescriptors)
{
    /// <summary>
    /// The topology this page refers to
    /// </summary>
    public DeviceTopologyName TopologyName { get; } = topologyName;

    /// <summary>
    /// A fully generic device for the topology, if it exists
    /// </summary>
    public IDeviceDescriptor? GenericDevice { get; } = deviceDescriptors?.FirstOrDefault(descriptor => descriptor.IsGeneric());

    /// <summary>
    /// A collection of devices for the topology
    /// </summary>
    public IReadOnlyList<IDeviceDescriptor> Devices { get; set; } = deviceDescriptors?.ToArray() ?? [];
}
