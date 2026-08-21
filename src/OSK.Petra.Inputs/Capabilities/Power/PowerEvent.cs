using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Capabilities.Power;

public readonly struct PowerEvent(PowerAxis axis, float power): IInputEvent
{
    public float Power { get; } = power;

    public PowerAxis Axis { get; } = axis;
}
