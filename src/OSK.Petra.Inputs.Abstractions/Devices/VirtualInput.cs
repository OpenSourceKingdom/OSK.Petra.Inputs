using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// A virtual input class that provides some implementation for <see cref="IVirtualInput"/>
/// </summary>
/// <typeparam name="TInput">The virtual input</typeparam>
public abstract class VirtualInput<TInput> : IVirtualInput
    where TInput: IVirtualInput
{
    #region IVirtualInput

    public bool Equals(IVirtualInput other)
        => other is TInput typed && Equals(typed);

    public abstract Task<IEnumerable<InputGlyph>> GetGlyphsAsync(DeviceCatalog deviceCatalog, CancellationToken cancellationToken = default);

    #endregion

    #region Helpers

    protected abstract bool Equals(TInput other);

    #endregion
}
