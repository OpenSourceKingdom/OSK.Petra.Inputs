using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class InputSystemBuilder : IInputSystemBuilder
{
    #region Variables

    private Type? _schemeRepositoryType;
    private readonly Dictionary<string, ActionDefinition> _definitionLookup = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<DeviceTopologyName, IDeviceTopology> _topologyLookup = [];

    /// <summary>
    /// Lookup keys for the schemes:
    ///   - InputConfigurationId
    ///   - DefinitionId
    ///   - Scheme Name
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, InputScheme>>> _schemeLookup = [];
    private Action<InputSystemJoinPolicy>? _joinPolicyConfigurator;

    #endregion

    #region IInputSystemConfigurationBuilder

    public IInputSystemBuilder UseSchemeRepository<T>() 
        where T : ISchemeRepository
    {
        _schemeRepositoryType = typeof(T);
        return this;
    }

    public IInputSystemBuilder WithActionDefinition(ActionDefinition definition)
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

        return this;
    }

    public IInputSystemBuilder WithDeviceTopology(IDeviceTopology topology)
    {
        if (topology is null)
        {
            throw new ArgumentNullException(nameof(topology), "Topology definitions can not be null.");
        }
        if (_topologyLookup.TryGetValue(topology.Name, out _))
        {
            throw new InvalidOperationException($"Topology definitions must be unique and '{topology.Name}' was already added.");
        }

        _topologyLookup[topology.Name] = topology;
        return this;
    }

    public IInputSystemBuilder WithInputScheme(InputScheme scheme)
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

    public IInputSystemBuilder WithJoinPolicy(Action<InputSystemJoinPolicy> policyConfigurator)
    {
        if (policyConfigurator is null)
        {
            throw new ArgumentNullException(nameof(policyConfigurator));
        }

        _joinPolicyConfigurator = policyConfigurator;

        return this;
    }

    #endregion

    #region Helpers

    internal Type? ScheemRepositoryType => _schemeRepositoryType;

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

        var joinPolicy = new InputSystemJoinPolicy()
        {
            MaxUsers = 1,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation
        };
        _joinPolicyConfigurator?.Invoke(joinPolicy);

        return new InputSystemConfiguration(_topologyLookup.Values, inputConfigurations, _definitionLookup.Values, joinPolicy);
    }

    #endregion
}
