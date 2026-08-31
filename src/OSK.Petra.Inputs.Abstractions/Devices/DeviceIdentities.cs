namespace OSK.Petra.Inputs.Abstractions.Devices;

public static class DeviceIdentities
{
    public const string GenericDeviceName = "Generic";

    public static readonly DeviceIdentity GenericGamepad = Gamepad(GenericDeviceName);
    public static DeviceIdentity Gamepad(string deviceName, DeviceFamily? family = null)
        => new(DeviceTopologyName.Gamepad, family ?? DeviceFamily.Generic, deviceName);

    public readonly static DeviceIdentity GenericKeyboard = Keyboard(GenericDeviceName);
    public static DeviceIdentity Keyboard(string deviceName, DeviceFamily? family = null)
        => new(DeviceTopologyName.Keyboard, family ?? DeviceFamily.Generic, deviceName);

    public static readonly DeviceIdentity GenericMouse = Mouse(GenericDeviceName);
    public static DeviceIdentity Mouse(string deviceName, DeviceFamily? family = null)
        => new(DeviceTopologyName.Mouse, family ?? DeviceFamily.Generic, deviceName);
}
