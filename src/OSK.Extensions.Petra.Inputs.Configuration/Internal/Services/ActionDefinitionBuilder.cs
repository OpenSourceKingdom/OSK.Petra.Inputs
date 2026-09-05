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
    private readonly Dictionary<string, Dictionary<string, InputSchemeBuilder>> _schemeBuilders = [];

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

        var schemeBuilder = new InputSchemeBuilder(name);
        configurator(schemeBuilder);

        var configurationId = InputConfiguration.GetConfigurationId(schemeBuilder.GetDeviceIdentities());
        if (!_schemeBuilders.TryGetValue(configurationId, out var schemeBuilderLookup))
        {
            schemeBuilderLookup = new(StringComparer.OrdinalIgnoreCase);
            _schemeBuilders[configurationId] = schemeBuilderLookup;
        }

        if (schemeBuilderLookup.TryGetValue(name, out _))
        {
            throw new InvalidOperationException($"Unable to create the scheme '{name}' for input configuration '{string.Join(",", schemeBuilder.GetDeviceIdentities())}' because a scheme with that name already exists for the configuration.");
        }

        schemeBuilderLookup[name] = schemeBuilder;
        return this;
    }

    #endregion

    #region Helpers

    internal (ActionDefinition Definition, IEnumerable<InputScheme> Schemes) Build()
    {
        var definition = new ActionDefinition(definitionName, _actionLookup.Values, _isDefault);

        var schemes = new List<InputScheme>();
        foreach (var builder in _schemeBuilders.Values.SelectMany(builderLookup => builderLookup.Values))
        {
            schemes.Add(builder.Build(definition));
        }

        return (definition, schemes);
    }

    #endregion
}
