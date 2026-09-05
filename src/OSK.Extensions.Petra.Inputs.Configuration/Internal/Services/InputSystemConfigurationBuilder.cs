using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class InputSystemConfigurationBuilder : IInputSystemConfigurationBuilder
{
    #region Variables

    private readonly Dictionary<string, ActionDefinition> _definitionLookup = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Lookup keys for the schemes:
    ///   - InputConfigurationId
    ///   - DefinitionId
    ///   - Scheme Name
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, InputScheme>>> _schemeLookup = [];

    private readonly Dictionary<Type, CapabilityOptions> _capabilityOptions = [];

    private InputSystemJoinPolicy? _joinPolicy;

    #endregion

    #region IInputSystemConfigurationBuilder

    public IInputSystemConfigurationBuilder WithActionDefinition(ActionDefinition definition)
    {
        if (definition is null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            throw new InvalidOperationException($"Definition name can not be empty");
        }
        if (_definitionLookup.TryGetValue(definition.Name, out _))
        {
            throw new InvalidOperationException($"An action definition '{definition.Name}' already exists.");
        }

        _definitionLookup[definition.Name] = definition;

        return this;
    }

    public IInputSystemConfigurationBuilder WithInputScheme(InputScheme scheme)
    {
        if (scheme is null)
        {
            throw new ArgumentNullException(nameof(scheme));
        }

        if (string.IsNullOrWhiteSpace(scheme.Name))
        {
            throw new InvalidOperationException("Scheme name can not be mepty.");
        }
        if (string.IsNullOrWhiteSpace(scheme.DefinitionName))
        {
            throw new InvalidOperationException("Scheme definition name can not be empty.");
        }

        var configurationId = InputConfiguration.GetConfigurationId(scheme.GetDeviceIdentities());
        if (!_schemeLookup.TryGetValue(configurationId, out var configurationSchemeLookup))
        {
            configurationSchemeLookup = new(StringComparer.OrdinalIgnoreCase);
            _schemeLookup[configurationId] = configurationSchemeLookup;
        }
        if (!configurationSchemeLookup.TryGetValue(scheme.DefinitionName, out var definitionSchemeLookup))
        {
            definitionSchemeLookup = new(StringComparer.OrdinalIgnoreCase);
            configurationSchemeLookup[scheme.DefinitionName] = definitionSchemeLookup;
        }

        if (definitionSchemeLookup.TryGetValue(scheme.Name, out _))
        {
            var deviceNames = string.Join(",", scheme.GetDeviceIdentities());
            throw new InvalidOperationException($"Unable to add scheme '{scheme.Name}' for definition '{scheme.DefinitionName}' that uses devices '{deviceNames}' because a scheme with the name already exists.");
        }

        definitionSchemeLookup[scheme.Name] = scheme;

        return this;
    }

    public IInputSystemConfigurationBuilder WithJoinPolicy(InputSystemJoinPolicy joinPolicy)
    {
        if (joinPolicy is null)
        {
            throw new ArgumentNullException(nameof(joinPolicy));
        }

        _joinPolicy = joinPolicy;

        return this;
    }

    public IInputSystemConfigurationBuilder WithCapabilityOptions<TOptions>(Action<TOptions> optionsConfigurator)
        where TOptions : CapabilityOptions, new()
    {
        if (optionsConfigurator is null)
        {
            throw new ArgumentNullException(nameof(optionsConfigurator));
        }

        var options = new TOptions();
        optionsConfigurator(options);

        _capabilityOptions[typeof(TOptions)] = options;

        return this;
    }

    #endregion

    #region Helpers

    internal InputSystemConfiguration BuildConfiguration()
    {
        var inputConfigurations = _schemeLookup.Select(schemeKvp =>
        {
            var configuration = new InputConfiguration(schemeKvp.Value.First().Value.Values.First().GetDeviceIdentities().Select(identity => identity.TopologyName));

            foreach (var scheme in  schemeKvp.Value.Values.SelectMany(definitionSchemeLookup => definitionSchemeLookup.Values))
            {
                configuration.AddScheme(scheme);
            }

            return configuration;
        });

        var joinPolicy = _joinPolicy ?? new InputSystemJoinPolicy()
        {
            MaxUsers = 1,
            DevicePairingBehavior = DevicePairingBehavior.Balanced,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation
        };

        var capabilityOptionConfiguration = new InputCapabilityOptionConfiguration(_capabilityOptions.Values);

        return new InputSystemConfiguration(inputConfigurations, _definitionLookup.Values, joinPolicy, capabilityOptionConfiguration);
    }

    #endregion
}
