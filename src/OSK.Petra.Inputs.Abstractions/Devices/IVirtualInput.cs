using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Represents an input that is not hardware/device specific. That is, something purely software based.
/// </summary>
/// <remarks>
/// 💡Notes:
/// <list type="bullet">
/// <item>Virtual inputs must implement equality comparison to detect duplicates  when building schemes</item>
/// </list>
/// </remarks>
public interface IVirtualInput: IInput, IEquatable<IVirtualInput>
{
    /// <summary>
    /// Gets the visual glyph information to display to a user for this virtual input.
    /// </summary>
    /// <param name="deviceCatalog">The catalog of available devices used to resolve glyphs for combined inputs</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A collection of glyphs representing this virtual input visually</returns>
    Task<IEnumerable<InputGlyph>> GetGlyphsAsync(DeviceCatalog deviceCatalog, CancellationToken cancellationToken = default);
}
