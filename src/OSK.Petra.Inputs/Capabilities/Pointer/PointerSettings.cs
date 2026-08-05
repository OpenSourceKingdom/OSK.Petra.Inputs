using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class PointerSettings: InputSettings
{
    /// <summary>
    /// Defines the distance a pointer must be moved to be considered a valid 'move'
    /// </summary>
    public float DistanceThresholdd { get; set; }
}
