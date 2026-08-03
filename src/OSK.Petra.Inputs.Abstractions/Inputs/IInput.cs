namespace OSK.Petra.Inputs.Abstractions.Inputs;

/// <summary>
/// An input for an <see cref="DeviceSpecification"/>
/// </summary>
public interface IInput
{
    /// <summary>
    /// The device id for this input
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Gets the glyph information to show a user
    /// </summary>
    /// <returns>The glyph for the input</returns>
    InputGlyph GetGlyph();
}
