namespace OSK.Petra.Inputs.Abstractions.Inputs;

/// <summary>
/// Represents an entire identity of a device. This data is useful for UI or other purposes
/// </summary>
/// <param name="TopologyName">The topology of the device</param>
/// <param name="DeviceFamily">The associated device family</param>
/// <param name="Name">The real name of the device</param>
public readonly record struct DeviceIdentity(DeviceTopologyName TopologyName, DeviceFamily DeviceFamily, string Name)
{
}
