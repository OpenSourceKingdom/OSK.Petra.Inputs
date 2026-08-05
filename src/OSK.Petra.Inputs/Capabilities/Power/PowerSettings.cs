using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Capabilities.Power;

public class PowerSettings: InputSettings
{
    /// <summary>
    /// Whether this input can be reactivated during a particular time period
    /// </summary>
    public bool AllowReactivation { get; set; }

    /// <summary>
    /// The amount of power required for the particular input to be considered activate
    /// </summary>
    public float ActivationSensitivityThreshold { get; set; }
}
