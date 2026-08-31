using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Capabilities.Power;

public readonly struct PowerEvent(PowerAxis axis, float power): IInputEvent
{
    #region Static

    public static PowerEvent Full()
        => new(PowerAxis.Neutral, 1);

    public static PowerEvent Full(PowerAxis axis)
        => new(axis, 1);

    public static PowerEvent Zero()
        => new(PowerAxis.Neutral, 0);

    public static PowerEvent Zero(PowerAxis axis)
        => new(axis, 0);

    public static PowerEvent Activate(PowerAxis axis, float power)
        => new(axis, power < 0 ? 0 : power > 1 ? 1 : power);

    #endregion

    #region Variables

    public float Power { get; } = power;

    public PowerAxis Axis { get; } = axis;

    #endregion
}
