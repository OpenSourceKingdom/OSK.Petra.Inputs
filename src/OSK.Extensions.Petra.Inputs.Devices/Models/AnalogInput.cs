using OSK.Petra.Inputs.Capabilities.Power;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

/// <summary>
/// Represents an analog input with power axis settings (e.g., trigger, thumbstick).
/// </summary>
public abstract class AnalogInput : DeviceInput, IInput<PowerSettings>
{
    #region Constructors

    /// <summary>
    /// Initializes an analog input with default sensitivity threshold of 0.1.
    /// </summary>
    /// <param name="id">The unique input ID</param>
    /// <param name="axis">The power axis this analog input represents</param>
    public AnalogInput(long id, PowerAxis axis)
        : this(id, axis, .1f, false)
    {
    }

    /// <summary>
    /// Initializes an analog input with a custom sensitivity threshold.
    /// </summary>
    /// <param name="id">The unique input ID</param>
    /// <param name="axis">The power axis this analog input represents</param>
    /// <param name="powerSensitivtyThreshold">The minimum power threshold for activation</param>
    public AnalogInput(long id, PowerAxis axis, float powerSensitivtyThreshold)
        : this(id, axis, powerSensitivtyThreshold, false)
    {
    }

    /// <summary>
    /// Initializes an analog input with full configuration options.
    /// </summary>
    /// <param name="id">The unique input ID</param>
    /// <param name="axis">The power axis this analog input represents</param>
    /// <param name="powerSensitivtyThreshold">The minimum power threshold for activation</param>
    /// <param name="allowReactivation">Whether the input can be reactivated after deactivation</param>
    public AnalogInput(long id, PowerAxis axis, float powerSensitivtyThreshold, bool allowReactivation)
        : base(id)
    {
        Settings = new()
        {
            AllowReactivation = allowReactivation,
            PowerSensitivityThreshold = powerSensitivtyThreshold,
        };
    }

    #endregion

    #region IInput

    /// <summary>
    /// Gets the power settings for this analog input.
    /// </summary>
    public PowerSettings Settings { get; private set; }

    #endregion

    #region Api

    /// <summary>
    /// Gets the power axis this analog input represents.
    /// </summary>
    public PowerAxis Axis { get; }

    /// <summary>
    /// Updates the power sensitivity threshold for this input.
    /// </summary>
    /// <param name="sensitivity">The new sensitivity threshold (0-1)</param>
    public void SetSensitivityThreshold(float sensitivity)
        => Settings = new()
        {
            AllowReactivation = Settings.AllowReactivation,
            PowerSensitivityThreshold = sensitivity
        };

    /// <summary>
    /// Enables or disables input reactivation for this analog input.
    /// </summary>
    /// <param name="allow">Whether reactivation is allowed</param>
    public void AllowReactivation(bool allow)
        => Settings = new()
        {
            AllowReactivation = allow,
            PowerSensitivityThreshold = Settings.PowerSensitivityThreshold
        };

    #endregion
}
