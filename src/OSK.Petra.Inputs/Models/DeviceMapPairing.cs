using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Models;

/// <summary>
/// Represents a pairing of a device with a collection of items (input maps or device inputs).
/// </summary>
/// <typeparam name="T">
/// The type of items being paired with the device
/// </typeparam>
public readonly struct DeviceMapPairing<T>(DeviceIdentity deviceIdentity, IEnumerable<T> items)
{
    /// <summary>
    /// Gets the device identity associated with this pairing.
    /// </summary>
    public DeviceIdentity DeviceIdentity => deviceIdentity;

    /// <summary>
    /// Gets the collection of items paired with the device.
    /// </summary>
    public IReadOnlyList<T> Items { get; } = items is null ? [] : [.. items];
}
