using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Capabilities.Power;

/// <summary>
/// Configuration settings for power based input capabilities (e.g., buttons, triggers).
/// </summary>
public class PowerSettings: IInputSettings
{
    #region Variables

    /// <summary>
    /// Gets or sets whether this input can be reactivated within a particular time period after activation.
    /// </summary>
    public bool AllowReactivation { get; init; } = true;

    /// <summary>
    /// Gets or sets the power threshold required for this input to be considered intentionally activated.
    /// </summary>
    public float PowerSensitivityThreshold { get; init; } = .1f;

    #endregion
}
