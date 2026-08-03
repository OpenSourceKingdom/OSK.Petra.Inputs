using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Models;

public readonly struct DeviceMapPairing<T>(DeviceIdentity deviceIdentity, IEnumerable<T> items)
{
    public DeviceIdentity DeviceIdentity => deviceIdentity;

    public IReadOnlyList<T> Items { get; } = [.. items];
}
