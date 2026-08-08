using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class ActionDefinitionBuilder(string definitionName) : IActionDefinitionBuilder
{
    #region Variables

    private bool _isDefault;

    private readonly Dictionary<string, InputAction> _actionLookup = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Scheme lookup keys:
    ///  - InputConfigurationId
    ///  - Scheme Name
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, InputScheme>> _schemeLookup = [];

    #endregion

    #region IActionDefinitionBuilder

    public IActionDefinitionBuilder MakeDefault()
    {
        _isDefault = true;

        return this;
    }

    public IActionDefinitionBuilder WithAction(InputAction action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (string.IsNullOrWhiteSpace(action.Name))
        {
            throw new ArgumentNullException(nameof(action.Name), "Action name can not be empty");
        }
        if (_actionLookup.TryGetValue(action.Name, out _))
        {
            throw new InvalidOperationException($"An action with the name '{action.Name}' has already been added for the action definition '{definitionName}'");
        }

        if (action.ActionExecutor is null)
        {
            throw new ArgumentNullException(nameof(action.ActionExecutor), $"The action '{action.Name}' for action definition '{definitionName}' has no executor can not be used.");
        }
        if (action.TriggerPhases is null)
        {
            throw new ArgumentNullException(nameof(action.TriggerPhases), $"The action '{action.Name}' for action definition '{definitionName}' has no trigger phases and can not be used.");
        }
        if (!action.TriggerPhases.Any())
        {
            throw new InvalidOperationException($"The action '{action.Name}' for action definition '{definitionName}' has no trigger phases and can not be used.");
        }

        _actionLookup[action.Name] = action;

        return this;
    }

    public IActionDefinitionBuilder WithScheme(string name, Action<IInputSchemeBuilder> configurator)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "The scheme name can not be null.");
        }
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        var schemeBuilder = new InputSchemeBuilder(definitionName, name);
        configurator(schemeBuilder);

        var newScheme = schemeBuilder.Build();
        var configurationId = InputConfiguration.GetConfigurationId(newScheme.GetDeviceIdentities());
        if (_schemeLookup.TryGetValue(configurationId, out var configurationSchemeLookup) && configurationSchemeLookup.TryGetValue(name, out _))
        {
            throw new InvalidOperationException($"Unable to create the scheme '{name}' for input configuration '{string.Join(",", newScheme.GetDeviceIdentities())}' because a scheme with that name already exists for the configuration.");
        }

        configurationSchemeLookup[name] = newScheme;
        return this;
    }

    #endregion

    #region Helpers

    internal IEnumerable<InputScheme> GetInputSchemes()
        => _schemeLookup.Values.SelectMany(v => v.Values);

    internal ActionDefinition Build()
        => new ActionDefinition(definitionName, _actionLookup.Values, _isDefault);

    #endregion
}
