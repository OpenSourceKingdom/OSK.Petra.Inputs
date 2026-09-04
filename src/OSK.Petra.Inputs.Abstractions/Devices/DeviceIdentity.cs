namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Represents an entire identity of a device. This data is useful for UI or other purposes
/// </summary>
/// <param name="TopologyName">The topology of the device</param>
/// <param name="DeviceFamily">The associated device family</param>
/// <param name="Name">The real name of the device</param>
public readonly record struct DeviceIdentity(DeviceTopologyName TopologyName, DeviceFamily DeviceFamily, string Name)
{
    /// <summary>
    /// Creates a generic device identity for the topology 
    /// </summary>
    /// <param name="topologyName">The topology name for the device identity</param>
    public DeviceIdentity(DeviceTopologyName topologyName)
        : this(topologyName, DeviceFamily.Generic, DeviceIdentities.GenericDeviceName)
    { 
    }

    /// <summary>
    /// Creates a generic device identity for the topoloy and family
    /// </summary>
    /// <param name="topologyName">The topology name for the device</param>
    /// <param name="deviceFamily">The family the device belongs to</param>
    public DeviceIdentity(DeviceTopologyName topologyName, DeviceFamily deviceFamily)
        : this(topologyName, deviceFamily, DeviceIdentities.GenericDeviceName)
    {
    }
}
