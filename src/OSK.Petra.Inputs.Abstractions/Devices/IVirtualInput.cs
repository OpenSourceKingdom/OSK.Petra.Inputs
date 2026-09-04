using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public interface IVirtualInput: IInput, IEquatable<IVirtualInput>
{
    /// <summary>
    /// Gets the glyph information to show a user
    /// </summary>
    /// <returns>The glyph for the input</returns>
    Task<IEnumerable<InputGlyph>> GetGlyphsAsync(DeviceCatalog deviceCatalog, CancellationToken cancellationToken = default);
}
