using OSK.Petra.Inputs.Capabilities.Power;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class AnalogInput : Input, IInput<PowerSettings>
{
    #region Constructors

    public AnalogInput(long id, PowerAxis axis)
        : this(id, axis, .1f, false)
    {
    }

    public AnalogInput(long id, PowerAxis axis, float powerSensitivtyThreshold)
        : this(id, axis, powerSensitivtyThreshold, false)
    {
    }

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

    public PowerSettings Settings { get; private set; }

    #endregion

    #region Api

    public PowerAxis Axis { get; }

    public void SetSensitivityThreshold(float sensitivity)
        => Settings = new()
        {
            AllowReactivation = Settings.AllowReactivation,
            PowerSensitivityThreshold = sensitivity
        };

    public void AllowReactivation(bool allow)
        => Settings = new()
        {
            AllowReactivation = allow,
            PowerSensitivityThreshold = Settings.PowerSensitivityThreshold
        };

    #endregion
}
