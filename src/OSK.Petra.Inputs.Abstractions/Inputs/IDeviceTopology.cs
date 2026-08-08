using System.Collections.Generic;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

/// <summary>
/// Describes validation and filtering rules for a device topology. It defines what families are supported
/// and provides matching and factory helpers for generic devices.
/// </summary>
public interface IDeviceTopology
{
    /// <summary>
    /// The name of the topology this represents
    /// </summary>
    DeviceTopologyName Name { get; }

    /// <summary>
    /// The set of device families that this topology supports.
    /// If null, all device families are supported.
    /// </summary>
    IReadOnlyCollection<DeviceFamily>? SupportedDeviceFamilies { get; }

    /// <summary>
    /// Determines whether the specified input matches the expectations for this topology.
    /// </summary>
    /// <param name="input">The input instance to check</param>
    /// <returns>True if the input is compatible with this topology; otherwise false.</returns>
    bool IsCompatibleInput(IInput input);

    /// <summary>
    /// Creates a generic device input map for a default device for this topology.
    /// This can be used when a concrete device is not available.
    /// </summary>
    /// <returns>A  representing a generic device for this topology</returns>
    IDeviceDescriptor CreateGeneric();
}
