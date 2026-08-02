using OSK.Petra.Inputs.Abstractions.Inputs;
using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public abstract class InputState
{
    public InputPhase Phase { get; set; } = InputPhase.Start;

    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
}
