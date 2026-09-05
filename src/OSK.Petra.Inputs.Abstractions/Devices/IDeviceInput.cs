using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// An input for an <see cref="IDeviceDescriptor"/>
/// </summary>
public interface IDeviceInput: IInput
{
    /// <summary>
    /// The device id for this input
    /// </summary>
    long Id { get; }

    /// <summary>
    /// Gets the glyph information to show a user
    /// </summary>
    /// <returns>The glyph for the input</returns>
    Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default);
}
