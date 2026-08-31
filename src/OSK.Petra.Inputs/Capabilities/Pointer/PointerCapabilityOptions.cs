using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class PointerCapabilityOptions: CapabilityOptions
{
    /// <summary>
    /// Sets the maximum number of entries any particular pointer will track beyond the current position.
    /// </summary>
    public int MaxPositionEntries { get; set; }
}
