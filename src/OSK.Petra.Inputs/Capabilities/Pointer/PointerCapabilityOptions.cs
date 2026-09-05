using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// Capability options to adjust processing pointers
/// </summary>
public class PointerCapabilityOptions: CapabilityOptions
{
    /// <summary>
    /// Sets the maximum number of entries any particular pointer will track beyond the current position.
    /// </summary>
    public int MaxPositionEntries { get; set; }
}
