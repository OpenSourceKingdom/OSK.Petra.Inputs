using OSK.Petra.Inputs.Abstractions.Inputs;
using System;

namespace OSK.Petra.Inputs.Capabilities.Power;

public class PowerCapabilityOptions
{
    /// <summary>
    /// The amount of time the input phase may remain inactive before the interaction
    /// is considered fully ended. If the input is reactivated within this time window,
    /// it is treated as a continuation of the same interaction rather than a new one.
    /// </summary>
    public TimeSpan? ReactivationTime { get; set; }

    /// <summary>
    /// The minimum amount of time an input must remain in the <see cref="InputPhase.Start"/>
    /// phase before transitioning to <see cref="InputPhase.Active"/>.
    /// </summary>
    public TimeSpan ActiveTimeThreshold { get; set; }
}
