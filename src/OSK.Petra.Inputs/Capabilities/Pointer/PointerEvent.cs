using OSK.Petra.Inputs.Abstractions.Runtime;
using System.Numerics;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public readonly struct PointerEvent(Vector2 position): IInputEvent
{
    public Vector2 Position => position;
}
