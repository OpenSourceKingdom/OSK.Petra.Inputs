using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Defines a set of actions that are available for the input system along with the <see cref="InputScheme"/>s that provide mappings for those actions
/// </summary>
/// <param name="name">The name for the definition</param>
/// <param name="actions">The list of actions</param>
/// <param name="schemes">The input schemes the definition will use</param>
/// <param name="isDefault">If this is the default definition to use</param>
public class ActionDefinition(string name, IEnumerable<InputAction> actions, IEnumerable<InputScheme> schemes, bool isDefault)
{
    #region Variables

    private readonly Dictionary<string, InputAction> _actionLookup
        = actions?.Where(action => action?.Name is not null).ToDictionary(action => action.Name, StringComparer.OrdinalIgnoreCase) ?? [];

    #endregion

    #region Api

    public string Name => name;

    public bool IsDefault => isDefault;

    public IReadOnlyCollection<InputAction> Actions => _actionLookup.Values;
    
    public InputAction? GetAction(string name)
        => !string.IsNullOrWhiteSpace(name) && _actionLookup.TryGetValue(name, out var action)
            ? action
            : null;

    #endregion
}
