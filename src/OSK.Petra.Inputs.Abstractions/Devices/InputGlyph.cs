namespace OSK.Petra.Inputs.Abstractions.Devices;

public class InputGlyph
{
    public required DeviceIdentity DeviceIdentity { get; set; }

    public required IDeviceInput Input { get; init; }

    public required string Text { get; init; }
}
