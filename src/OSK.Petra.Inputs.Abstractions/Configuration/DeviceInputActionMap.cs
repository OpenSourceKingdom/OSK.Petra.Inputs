using System;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents a mapping between an action and a concrete input instance for a device map.
/// This is used in configuration-level device maps and schemes.
/// </summary>
public readonly struct DeviceInputActionMap
{
    #region Variables

    /// <summary>
    /// The action name this maps to
    /// </summary>
    public InputAction Action { get; init; }

    /// <summary>
    /// The input this maps to
    /// </summary>
    public long InputId { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an input action map for a device
    /// </summary>
    /// <param name="action">The <see cref="InputAction"/> this map uses</param>
    /// <param name="inputId">The device input that is associated with this map</param>
    /// <exception cref="ArgumentNullException">If the action is null</exception>
    public DeviceInputActionMap(InputAction action, long inputId)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        InputId = inputId;
    }

    #endregion
}
