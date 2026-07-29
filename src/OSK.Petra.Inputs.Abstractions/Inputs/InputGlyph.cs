namespace OSK.Petra.Inputs.Abstractions.Inputs;

public class InputGlyph
{
    public required DeviceIdentity DeviceIdentity { get; set; }

    public required IInput Input { get; init; }

    public required string Symbol { get; init; }
}
