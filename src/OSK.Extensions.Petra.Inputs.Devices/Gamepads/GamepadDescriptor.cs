using OSK.Petra.Inputs.Abstractions.Devices;
using System.Collections.Generic;

namespace OSK.Extensions.Petra.Inputs.Devices.Gamepads;

/// <summary>
/// Represents a Gamepad device
/// </summary>
public class GamepadDescriptor: DeviceDescriptor<IGamepadInput>
{
    #region Constructors

    /// <summary>
    /// Create a generic gamepad
    /// </summary>
    /// <param name="gamepadInputs">The available gamepad inputs</param>
    public GamepadDescriptor(IEnumerable<IGamepadInput> gamepadInputs)
        : this(DeviceFamily.Generic, gamepadInputs)
    {
    }

    /// <summary>
    /// Creates a generic gamepad for the famly
    /// </summary>
    /// <param name="family">The device family</param>
    /// <param name="gamepadInputs">The available gamepad inputs</param>
    public GamepadDescriptor(DeviceFamily family, IEnumerable<IGamepadInput> gamepadInputs)
        : this(family, DeviceIdentities.GenericDeviceName, gamepadInputs)
    {
    }

    /// <summary>
    /// Creates a gamepad
    /// </summary>
    /// <param name="family">The family for the gamepad</param>
    /// <param name="deviceName">The device name</param>
    /// <param name="gamepadInputs">The available gamepad inputs</param>
    public GamepadDescriptor(DeviceFamily family, string deviceName, IEnumerable<IGamepadInput> gamepadInputs)
        : base(new(DeviceTopologyName.Gamepad, family, deviceName), gamepadInputs)
    {
    }

    #endregion
}
