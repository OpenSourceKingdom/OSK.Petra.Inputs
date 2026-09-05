using OSK.Petra.Inputs.Abstractions.Devices;
using System;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents a mapping between an action and a virtual input instance.
/// This is used in configuration-level device maps and schemes.
/// </summary>
public readonly struct VirtualInputActionMap
{
    #region Variables

    /// <summary>
    /// The action name this maps to
    /// </summary>
    public InputAction Action { get; init; }

    /// <summary>
    /// The virtual input this maps to
    /// </summary>
    public IVirtualInput VirtualInput { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an input action map for a virtual input
    /// </summary>
    /// <param name="action">The <see cref="InputAction"/> this map uses</param>
    /// <param name="input">The virtual input that is associated with this map</param>
    /// <exception cref="ArgumentNullException">If the action is null</exception>
    public VirtualInputActionMap(InputAction action, IVirtualInput input)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        VirtualInput = input ?? throw new ArgumentNullException(nameof(input));
    }

    #endregion
}
