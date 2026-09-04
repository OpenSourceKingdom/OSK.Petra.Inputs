using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// The very necessary configuration utilized with the input system. This is the 'source of truth' for all interactions 
/// and decisions made by the input system
/// </summary>
public class InputSystemConfiguration
{
    #region Variables

    private readonly HashSet<DeviceTopologyName> _topologyLookup;
    private readonly Dictionary<string, InputConfiguration> _inputConfigurationLookup;
    private readonly Dictionary<string, ActionDefinition> _inputDefinitionLookup;

    #endregion

    #region Constructors

    /// <param name="inputConfigurations">The configurations of topologies that is supported</param>
    /// <param name="definitions">The input definitions the input system will use to map inputs and actions</param>
    /// <param name="joinPolicy">The policy the input system uses for new users, devices, and the like</param>
    /// <param name="capabilityOptionConfiguration">The configuration containing custom options for the capabilities available in the input system</param>
    public InputSystemConfiguration(IEnumerable<InputConfiguration> inputConfigurations, IEnumerable<ActionDefinition> definitions, InputSystemJoinPolicy joinPolicy,
        InputCapabilityOptionConfiguration capabilityOptionConfiguration)
    {
        _inputConfigurationLookup = inputConfigurations?.Where(configuration => configuration is not null).ToDictionary(configuration => configuration.Id) ?? [];
        _inputDefinitionLookup = definitions?.Where(definition => definition?.Name is not null).ToDictionary(definition => definition.Name, StringComparer.OrdinalIgnoreCase) ?? [];
        _topologyLookup = inputConfigurations is null ? [] : [.. inputConfigurations.SelectMany(config => config.TopologyNames)];
        JoinPolicy = joinPolicy ?? throw new ArgumentNullException(nameof(joinPolicy));
        CapabilityConfiguration = capabilityOptionConfiguration ?? throw new ArgumentNullException(nameof(capabilityOptionConfiguration));
    }

    #endregion

    #region Api

    /// <summary>
    /// The policy that determines new user, device, etc. behaviors when interacting with the input system
    /// </summary>
    public InputSystemJoinPolicy JoinPolicy { get; }

    public IReadOnlyCollection<DeviceTopologyName> DeviceTopologies => [.. _topologyLookup];

    /// <summary>
    /// The collection of supported input device configurations for built in and custom schemes to use.
    /// </summary>
    public IReadOnlyList<InputConfiguration> InputConfigurations 
        => [.. _inputConfigurationLookup.Values];

    /// <summary>
    /// The collection of input definitions that are available for users
    /// </summary>
    public IReadOnlyList<ActionDefinition> Definitions
        => [.. _inputDefinitionLookup.Values];

    public InputConfiguration? GetInputConfiguration(string configurationId)
        => _inputConfigurationLookup.TryGetValue(configurationId, out var configuration)
            ? configuration
            : null;

    public bool IsTopologySupported(DeviceTopologyName topologyName)
        => _topologyLookup.TryGetValue(topologyName, out _);

    /// <summary>
    /// Attempts to get the definition from a name
    /// </summary>
    /// <param name="definitionName">The name of the definition to get</param>
    /// <returns>The definition if the name matches an existing definition in the input system, otherwise null</returns>
    public ActionDefinition? GetDefinition(string definitionName)
        => !string.IsNullOrWhiteSpace(definitionName) && _inputDefinitionLookup.TryGetValue(definitionName, out var definition)
            ? definition
            : null;

    public InputCapabilityOptionConfiguration CapabilityConfiguration { get; }

    #endregion
}
