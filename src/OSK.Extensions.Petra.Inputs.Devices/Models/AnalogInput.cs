using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class AnalogInput : Input, IInput<PowerSettings>
{
    #region Constructors

    public AnalogInput(int id)
        : this(id, .1f, false)
    {
    }

    public AnalogInput(int id, float powerSensitivtyThreshold)
        : this(id, powerSensitivtyThreshold, false)
    {
    }

    public AnalogInput(int id, float powerSensitivtyThreshold, bool allowReactivation)
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
