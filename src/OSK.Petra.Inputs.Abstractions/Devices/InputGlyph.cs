namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Represents the visual representation of an input for UI display.
/// </summary>
public class InputGlyph
{
    /// <summary>
    /// Gets the device this glyph represents.
    /// </summary>
    public required DeviceIdentity DeviceIdentity { get; set; }

    /// <summary>
    /// Gets the specific device input.
    /// </summary>
    public required IDeviceInput Input { get; init; }

    /// <summary>
    /// Gets the text or key name to display.
    /// </summary>
    public required string Text { get; init; }
}
