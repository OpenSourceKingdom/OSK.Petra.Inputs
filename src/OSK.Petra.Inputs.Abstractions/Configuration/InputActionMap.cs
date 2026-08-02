using OSK.Petra.Inputs.Abstractions.Inputs;
using System;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents a mapping between an action name and a concrete input instance for a device map.
/// This is used in configuration-level device maps and schemes.
/// </summary>
public readonly struct InputActionMap
{
    #region Variables

    /// <summary>
    /// The action name this maps to
    /// </summary>
    public string ActionName { get; init; }

    /// <summary>
    /// The input this maps to
    /// </summary>
    public IInput Input { get; init; }

    #endregion

    #region Constructors

    public InputActionMap(string actionName, IInput input)
    {
        ActionName = actionName ?? throw new ArgumentNullException(nameof(actionName));
        Input = input ?? throw new ArgumentNullException(nameof(input));
    }

    #endregion
}
