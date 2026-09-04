using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

/// <summary>
/// Represents a digital (on/off) input with power settings (e.g., buttons).
/// </summary>
public abstract class DigitalInput : DeviceInput, IPowerInput
{
    #region Constructors

    /// <summary>
    /// Initializes a digital input with default reactivation disabled.
    /// </summary>
    /// <param name="id">The unique input ID</param>
    protected DigitalInput(long id)
        : this(id, false) 
    {
    }

    /// <summary>
    /// Initializes a digital input with specified reactivation behavior.
    /// </summary>
    /// <param name="id">The unique input ID</param>
    /// <param name="allowReactivation">Whether the input can be reactivated after
    /// deactivation</param>
    protected DigitalInput(long id, bool allowReactivation)
        : base(id)
    {
        Settings = new PowerSettings()
        {
            AllowReactivation = allowReactivation,
            PowerSensitivityThreshold = 1
        };
    }

    #endregion

    #region IPowerInput

    /// <summary>
    /// Gets the power settings for this digital input.
    /// </summary>
    public PowerSettings Settings { get; }

    #endregion
}
