namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// A tool to help create different types of <see cref="DeviceIdentity"/>
/// </summary>
public static class DeviceIdentities
{
    /// <summary>
    /// A name that represents what is expected to be a generic device
    /// </summary>
    public const string GenericDeviceName = "Generic";

    /// <summary>
    /// A gamepad that is completely generic
    /// </summary>
    public static readonly DeviceIdentity GenericGamepad = Gamepad(DeviceFamily.Generic, GenericDeviceName);

    /// <summary>
    /// Creates different gamepad identities
    /// </summary>
    /// <param name="family">The family of gamepad</param>
    /// <param name="deviceName">The name of the device</param>
    /// <returns>The created identity</returns>
    public static DeviceIdentity Gamepad(DeviceFamily family, string deviceName)
        => new(DeviceTopologyName.Gamepad, family, deviceName);

    /// <summary>
    /// A keyboard that is completely generic
    /// </summary>
    public readonly static DeviceIdentity GenericKeyboard = Keyboard(DeviceFamily.Generic, GenericDeviceName);

    /// <summary>
    /// Creates different keyboard identities
    /// </summary>
    /// <param name="family">The family of keyboard</param>
    /// <param name="deviceName">The name of the device</param>
    /// <returns>The created identity</returns>
    public static DeviceIdentity Keyboard(DeviceFamily family, string deviceName)
        => new(DeviceTopologyName.Keyboard, family, deviceName);

    /// <summary>
    /// A mouse that is completely generic
    /// </summary>
    public static readonly DeviceIdentity GenericMouse = Mouse(DeviceFamily.Generic, GenericDeviceName);

    /// <summary>
    /// Creates different mouse identities
    /// </summary>
    /// <param name="family">The family of mouse</param>
    /// <param name="deviceName">The name of the device</param>
    /// <returns>The created identity</returns>
    public static DeviceIdentity Mouse(DeviceFamily family, string deviceName)
        => new(DeviceTopologyName.Mouse, family, deviceName);
}
