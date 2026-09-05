using OSK.Petra.Inputs.Abstractions.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Devices;

/// <summary>
/// A tool that helps with <see cref="IDeviceDescriptor"/>
/// </summary>
public static class DeviceHelper
{
    /// <summary>
    /// Generates an enum collection for a given enum type
    /// </summary>
    /// <typeparam name="TEnum">The enum type to get a collection for</typeparam>
    /// <returns>The collection of all enum values within the enum</returns>
    public static IEnumerable<TEnum> GetValues<TEnum>()
        where TEnum : Enum
        => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
}
