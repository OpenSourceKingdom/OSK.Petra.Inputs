namespace OSK.Petra.Inputs.Abstractions.Inputs;

/// <summary>
/// Represents a unique input device family that a collection of devices are associated with
/// </summary>
/// <param name="Name">The family of the device</param>
public readonly record struct DeviceFamily(string Name)
{
    public readonly static DeviceFamily Generic = new("GamePad");
    public readonly static DeviceFamily Xbox = new("Xbox");
    public readonly static DeviceFamily PlayStation = new("PlayStation");
    public readonly static DeviceFamily Nintendo = new("Nintendo");
    public readonly static DeviceFamily Steam = new("Steam");
}
