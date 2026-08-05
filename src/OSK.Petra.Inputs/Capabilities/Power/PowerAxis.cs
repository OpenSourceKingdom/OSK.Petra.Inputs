using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Capabilities.Power;

/// <summary>
/// Represents a specific axes of interaction for a given input. For example, 2D pointer motion interacts with 
/// an X and a Y direction, which is 2 axes of interaction. Button inputs would possess one axis, whereas inputs
/// like a joystick could possess two axis in the -1 to 1 range
/// 
/// The axes that can be interacted with on a particular input will depend on the device and input in question.
/// </summary>
/// <param name="name">A readable name for the axis</param>
/// <param name="id">The id the input interaction is associated with</param>
public readonly struct PowerAxis(string name, int id) : IEquatable<PowerAxis>
{
    #region Static

    /// <summary>
    /// A non-specific axes. 
    /// </summary>
    public static PowerAxis Neutral = new("Neutral", 0);

    /// <summary>
    /// Represents an axes in the X plane, horizontal direction.
    /// </summary>
    public static PowerAxis X = new("X - Horizontal", 1);
    /// <inheritdoc cref="X"/>
    public static PowerAxis Horizontal = X;
    /// <inheritdoc cref="X"/>
    public static PowerAxis One = X;


    /// <summary>
    /// Represents an axes in the Y plane, vertical direction.
    /// </summary>
    public static PowerAxis Y = new("Y - Vertical", 2);
    /// <inheritdoc cref="Y"/>
    public static PowerAxis Vertical = Y;
    /// <inheritdoc cref="Y"/>
    public static PowerAxis Two = Y;

    #endregion

    #region Variables

    /// <summary>
    /// The specific axis Id
    /// </summary>
    public int Id => id;

    /// <summary>
    /// A readable text for the axis, only meant to help distinguish the axis in a descriptive manner
    /// </summary>
    public string Name => name ?? string.Empty;

    #endregion

    #region Overrides

    public override string ToString()
    {
        return $"Axis: {Name}, {id}";
    }

    public override bool Equals(object obj)
    {
        return obj is PowerAxis other && Equals(other);
    }

    public override int GetHashCode()
    {
        return id;
    }

    public bool Equals(PowerAxis other)
    {
        return Id == other.Id;
    }

    #endregion

    #region Operators

    public static bool operator ==(PowerAxis one, PowerAxis two)
        => one.Equals(two);

    public static bool operator !=(PowerAxis one, PowerAxis two)
        => !one.Equals(two);

    #endregion
}
