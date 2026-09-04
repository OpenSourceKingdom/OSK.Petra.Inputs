using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Defines a set of actions that are available for the input system along with the <see cref="InputScheme"/>s that provide mappings for those actions
/// </summary>
/// <param name="name">The name for the definition</param>
/// <param name="actions">The list of actions</param>
/// <param name="isDefault">If this is the default definition to use</param>
public class ActionDefinition(string name, IEnumerable<InputAction> actions, bool isDefault)
{
    #region Variables

    private readonly Dictionary<string, InputAction> _actionLookup
        = actions?.Where(action => action?.Name is not null).ToDictionary(action => action.Name, StringComparer.OrdinalIgnoreCase) ?? [];

    #endregion

    #region Api

    /// <summary>
    /// The unique name of the definition in the input system.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Whether this definition is the default definition to use
    /// </summary>
    public bool IsDefault => isDefault;

    /// <summary>
    /// Gets all actions defined in this definition.
    /// </summary>
    public IReadOnlyCollection<InputAction> Actions => _actionLookup.Values;

    /// <summary>
    /// Retrieves an action by name
    /// </summary>
    /// <param name="name">The action name to retrieve</param>
    /// <returns>The action if found, otherwise null</returns>
    public InputAction? GetAction(string name)
        => !string.IsNullOrWhiteSpace(name) && _actionLookup.TryGetValue(name, out var action)
            ? action
            : null;

    #endregion
}
