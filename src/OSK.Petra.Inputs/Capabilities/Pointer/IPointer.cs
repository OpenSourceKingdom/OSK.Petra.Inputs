using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Numerics;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public interface IPointer: IInput<PointerSettings>
{
    Vector2 Position { get; }
}
