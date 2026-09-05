using OSK.Petra.Inputs.Abstractions.Runtime;
using System.Numerics;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// Represents an event that a pointer triggers
/// </summary>
/// <param name="position">The position of the pointer</param>
public readonly struct PointerEvent(Vector2 position): IInputEvent
{
    #region Variables

    /// <summary>
    /// The position of the pointer
    /// </summary>
    public Vector2 Position => position;

    #endregion
}
