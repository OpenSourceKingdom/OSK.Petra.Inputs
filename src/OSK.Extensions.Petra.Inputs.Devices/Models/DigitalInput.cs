using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class DigitalInput : Input, IPowerInput
{
    #region Constructors

    protected DigitalInput(long id)
        : this(id, false) 
    {
    }

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

    public PowerSettings Settings { get; }

    #endregion
}
