using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Keyboards;

/// <summary>
/// Represents a Keyboard device
/// </summary>
public abstract class KeyboardDescriptor: DeviceDescriptor<IKeyboardInput>
{
    #region Constructors

    /// <summary>
    /// Create a generic keyboard
    /// </summary>
    public KeyboardDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    /// <summary>
    /// Creates a generic keyboard for the famly
    /// </summary>
    /// <param name="family">The device family</param>
    public KeyboardDescriptor(DeviceFamily family)
        : this(family, DeviceIdentities.GenericDeviceName)
    {
    }

    /// <summary>
    /// Creates a keyboard
    /// </summary>
    /// <param name="family">The family for the keyboard</param>
    /// <param name="deviceName">The device name</param>
    public KeyboardDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Keyboard, family, deviceName))
    {
    }

    #endregion
}
