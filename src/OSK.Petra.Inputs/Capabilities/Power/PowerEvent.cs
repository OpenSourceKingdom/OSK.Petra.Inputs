using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Capabilities.Power;

/// <summary>
/// Represents an event that a power input triggers
/// </summary>
/// <param name="axis">The axis the power is applied tor</param>
/// <param name="power">The amount of applied power</param>
public readonly struct PowerEvent(PowerAxis axis, float power): IInputEvent
{
    #region Static

    /// <summary>
    /// Creates a full power event, agnostic of axis
    /// </summary>
    /// <returns>The created event</returns>
    public static PowerEvent Full()
        => new(PowerAxis.Neutral, 1);

    /// <summary>
    /// Creates a full power event, specifying the axis
    /// </summary>
    /// <param name="axis">The axis the power is applied to</param>
    /// <returns>The created event</returns>
    public static PowerEvent Full(PowerAxis axis)
        => new(axis, 1);

    /// <summary>
    /// Creates a power event with no force applied on any axis, i,e, the power source has stopped applying power
    /// </summary>
    /// <returns>The created event</returns>
    public static PowerEvent Zero()
        => new(PowerAxis.Neutral, 0);

    /// <summary>
    /// Creates a power event with no force applied on the specific axis, i,e, the power source has stopped applying power
    /// </summary>
    /// <returns>The created event</returns>
    public static PowerEvent Zero(PowerAxis axis)
        => new(axis, 0);

    /// <summary>
    /// Creates a power event with the specific power applied, agnostic of the axis
    /// </summary>
    /// <param name="power">The power applied</param>
    /// <returns>The created event</returns>
    public static PowerEvent Activate(float power)
        => Activate(PowerAxis.Neutral, power);

    /// <summary>
    /// Creates a power event with the specific power applied to the specific axis
    /// </summary>
    /// <param name="axis">The axis the power is applied to</param>
    /// <param name="power">The amount of power applied</param>
    /// <returns>The created event</returns>
    public static PowerEvent Activate(PowerAxis axis, float power)
        => new(axis, power < 0 ? 0 : power > 1 ? 1 : power);

    #endregion

    #region Variables

    /// <summary>
    /// The amount of power applied
    /// </summary>
    public float Power { get; } = power;

    /// <summary>
    /// The axis the power is applied to
    /// </summary>
    public PowerAxis Axis { get; } = axis;

    #endregion
}
