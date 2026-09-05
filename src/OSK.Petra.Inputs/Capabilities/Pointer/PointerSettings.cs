using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// Input settings for pointers
/// </summary>
public class PointerSettings: IInputSettings
{
    #region Variables

    /// <summary>
    /// Defines the distance a pointer must be moved to be considered a full, intentional movement
    /// </summary>
    public float DistanceThreshold { get; init; } = .1f;

    #endregion
}
