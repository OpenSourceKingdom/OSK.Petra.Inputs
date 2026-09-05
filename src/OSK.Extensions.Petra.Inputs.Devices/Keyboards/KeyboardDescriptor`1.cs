using OSK.Petra.Inputs.Abstractions.Devices;
using System.Collections.Generic;

namespace OSK.Extensions.Petra.Inputs.Devices.Keyboards;

/// <summary>
/// Represents a keyboard that uses <see cref="KeyboardKey"/>
/// </summary>
/// <typeparam name="TKey">The type of keyboard key</typeparam>
public class KeyboardDescriptor<TKey> : KeyboardDescriptor
    where TKey: KeyboardKey
{
    #region Constructors

    /// <summary>
    /// Create a generic keyboard
    /// </summary>
    /// <param name="keys">The available keys</param>
    public KeyboardDescriptor(IEnumerable<TKey> keys)
        : this(DeviceFamily.Generic, keys)
    {
    }

    /// <summary>
    /// Creates a generic keyboard for the famly
    /// </summary>
    /// <param name="family">The device family</param>
    /// <param name="keys">The available keys</param>
    public KeyboardDescriptor(DeviceFamily family, IEnumerable<TKey> keys)
        : this(family, DeviceIdentities.GenericDeviceName, keys)
    {
    }

    /// <summary>
    /// Creates a keyboard
    /// </summary>
    /// <param name="family">The family for the keyboard</param>
    /// <param name="deviceName">The device name</param>
    /// <param name="keys">The available keys</param>
    public KeyboardDescriptor(DeviceFamily family, string deviceName, IEnumerable<TKey> keys)
        : base(family, deviceName, keys)
    {
    }

    #endregion
}
