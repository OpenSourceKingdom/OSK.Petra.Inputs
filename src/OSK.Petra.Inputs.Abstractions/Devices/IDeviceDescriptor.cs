using System.Collections.Generic;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Describes a physical input device and its available inputs.
/// </summary>
public interface IDeviceDescriptor
{
    /// <summary>
    /// Gets the identity information for this device.
    /// </summary>
    DeviceIdentity Identity { get; }

    /// <summary>
    /// Gets all inputs available on this device.
    /// </summary>
    IReadOnlyCollection<IDeviceInput> Inputs { get; }

    /// <summary>
    /// Retrieves a specific input by ID.
    /// </summary>
    /// <param name="id">The id of the input to get</param>
    /// <returns>The input, if it exists, otherwise null</returns>
    IDeviceInput? GetInput(long id);
}
