using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Gamepads;

/// <summary>
/// Represents a Gamepad device
/// </summary>
public abstract class GamepadDescriptor: DeviceDescriptor<IGamepadInput>
{
    #region Constructors

    /// <summary>
    /// Create a generic gamepad
    /// </summary>
    public GamepadDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    /// <summary>
    /// Creates a generic gamepad for the famly
    /// </summary>
    /// <param name="family">The device family</param>
    public GamepadDescriptor(DeviceFamily family)
        : this(family, DeviceIdentities.GenericDeviceName)
    {
    }

    /// <summary>
    /// Creates a gamepad
    /// </summary>
    /// <param name="family">The family for the gamepad</param>
    /// <param name="deviceName">The device name</param>
    public GamepadDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Gamepad, family, deviceName))
    {
    }

    #endregion
}
