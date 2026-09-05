using OSK.Petra.Inputs.Abstractions.Devices;
using System.Collections.Generic;

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
    /// <param name="keyboardInputs">The available keyboard inputs</param>
    public KeyboardDescriptor(IEnumerable<IKeyboardInput> keyboardInputs)
        : this(DeviceFamily.Generic)
    {
    }

    /// <summary>
    /// Creates a generic keyboard for the famly
    /// </summary>
    /// <param name="family">The device family</param>
    /// <param name="keyboardInputs">The available keyboard inputs</param>
    public KeyboardDescriptor(DeviceFamily family, IEnumerable<IKeyboardInput> keyboardInputs)
        : this(family, DeviceIdentities.GenericDeviceName, keyboardInputs)
    {
    }

    /// <summary>
    /// Creates a keyboard
    /// </summary>
    /// <param name="family">The family for the keyboard</param>
    /// <param name="deviceName">The device name</param>
    /// <param name="keyboardInputs">The available keyboard inputs</param>
    public KeyboardDescriptor(DeviceFamily family, string deviceName, IEnumerable<IKeyboardInput> keyboardInputs)
        : base(new(DeviceTopologyName.Keyboard, family, deviceName), keyboardInputs)
    {
    }

    #endregion
}
