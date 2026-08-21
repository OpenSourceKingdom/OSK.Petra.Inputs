using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Capabilities.Power;

public class PowerSettings: IInputSettings
{
    #region Variables

    /// <summary>
    /// Whether this input can be reactivated during a particular time period
    /// </summary>
    public bool AllowReactivation { get; init; } = true;

    /// <summary>
    /// The amount of power required for the particular input to be considered activate
    /// </summary>
    public float PowerSensitivityThreshold { get; init; } = .1f;

    #endregion
}
