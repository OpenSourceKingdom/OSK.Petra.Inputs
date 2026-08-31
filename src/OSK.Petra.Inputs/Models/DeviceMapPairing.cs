using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Models;

public readonly struct DeviceMapPairing<T>(DeviceIdentity deviceIdentity, IEnumerable<T> items)
{
    public DeviceIdentity DeviceIdentity => deviceIdentity;

    public IReadOnlyList<T> Items { get; } = items is null ? [] : [.. items];
}
