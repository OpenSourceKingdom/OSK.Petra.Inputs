using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Power;

public class PowerDetails: ICapabilityDetails
{
    #region Variables

    public double Power { get; internal set; }

    public double Acceleration { get; internal set; }

    public int ActivationCount { get; internal set; }
    
    public PowerAxis Axis { get; internal set; }

    #endregion

    #region Api

    internal TimeSpan TimeSinceLastActivation { get; set; }

    #endregion
}
