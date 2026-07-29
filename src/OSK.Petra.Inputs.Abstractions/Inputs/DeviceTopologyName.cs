namespace OSK.Petra.Inputs.Abstractions.Inputs;

/// <summary>
/// The type of device an input device is
/// </summary>
public readonly record struct DeviceTopologyName(string Name)
{
    public static readonly DeviceTopologyName Keyboard = new("Keyboard");
    public static readonly DeviceTopologyName Mouse = new("Mouse");
    public static readonly DeviceTopologyName Gamepad = new("Gamepad");
}
