namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// Describes the movement of a pointer
/// </summary>
public enum PointerMovement
{
    /// <summary>
    /// The pointer has started interaction
    /// </summary>
    Start,

    /// <summary>
    /// The pointer is currently not moving
    /// </summary>
    Idle,

    /// <summary>
    /// The pointer is currently moving
    /// </summary>
    Active,

    /// <summary>
    /// The pointer has stopped interaction
    /// </summary>
    Stop
}
