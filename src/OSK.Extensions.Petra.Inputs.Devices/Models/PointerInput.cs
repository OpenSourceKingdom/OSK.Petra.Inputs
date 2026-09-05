using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

/// <summary>
/// Represents a pointer input with motion tracking settings (e.g., mouse movement).
/// </summary>
public abstract class PointerInput: DeviceInput, IInput<PointerSettings>
{
    #region Constructors

    /// <summary>
    /// Initializes a pointer input with default distance threshold of 0.1.
    /// </summary>
    /// <param name="id">The unique input ID</param>
    protected PointerInput(long id)
        : this(id, .1f)
    {
    }

    /// <summary>
    /// Initializes a pointer input with a custom distance threshold.
    /// </summary>
    /// <param name="id">The unique input ID</param>
    /// <param name="distanceThreshold">The minimum distance threshold for movement
    /// detection</param>
    protected PointerInput(long id, float distanceThreshold)
    : base(id)
    {
        Settings = new PointerSettings()
        {
            DistanceThreshold = distanceThreshold
        };
    }

    #endregion

    #region IInput

    /// <summary>
    /// Gets the pointer settings for this input.
    /// </summary>
    public PointerSettings Settings { get; private set; }

    #endregion

    #region Api

    /// <summary>
    /// Updates the distance threshold for movement detection.
    /// </summary>
    /// <param name="threshold">The new distance threshold (clamped >= 0)</param>
    public void SetDistanceThreshold(float threshold)
        => Settings = new()
        {
            DistanceThreshold = threshold < 0 ? 0 : threshold
        };

    #endregion
}
