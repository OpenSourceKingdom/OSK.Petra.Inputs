using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Power;

/// <summary>
/// Describes information relating to power data 
/// </summary>
public class PowerDetails: ICapabilityDetails
{
    #region Variables

    /// <summary>
    /// The amount of power applied to the input
    /// </summary>
    public double Power { get; internal set; }

    /// <summary>
    /// The acceleration of power being applied
    /// </summary>
    public double Acceleration { get; internal set; }

    /// <summary>
    /// Represents the total number of 'taps' of the input, within the alloted <see cref="PowerCapabilityOptions.ReactivationTime"/>
    /// </summary>
    public int ActivationCount { get; internal set; }
    
    /// <summary>
    /// The axis the power is applied
    /// </summary>
    public PowerAxis Axis { get; internal set; }

    #endregion

    #region Api

    internal TimeSpan TimeSinceLastActivation { get; set; }

    #endregion
}
