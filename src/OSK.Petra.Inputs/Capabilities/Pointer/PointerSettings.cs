using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class PointerSettings: IInputSettings
{
    #region Variables

    /// <summary>
    /// Defines the distance a pointer must be moved to be considered a valid 'move'
    /// </summary>
    public float DistanceThreshold { get; init; } = .1f;

    #endregion
}
