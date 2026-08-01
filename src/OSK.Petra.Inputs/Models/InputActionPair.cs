using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Models;

public readonly struct InputActionPair
{
    public InputAction Action { get; init; }

    public IInput Input { get; init; }
}
