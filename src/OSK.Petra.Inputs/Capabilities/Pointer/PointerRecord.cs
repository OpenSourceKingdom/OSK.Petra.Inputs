using System;
using System.Numerics;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

internal readonly struct PointerRecord(Vector2 position, TimeSpan time)
{
    public Vector2 Position => position;

    public TimeSpan Time => time;
}
