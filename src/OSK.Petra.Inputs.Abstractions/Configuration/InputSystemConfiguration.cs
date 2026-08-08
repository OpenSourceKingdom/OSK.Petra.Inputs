using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// The very necessary configuration utilized with the input system. This is the 'source of truth' for all interactions 
/// and decisions made by the input system
/// </summary>
/// <param name="deviceTopologies">The device topologies the input system is able to support</param>
/// <param name="supportedConfigurations">The configurations of topologies that is supported</param>
/// <param name="definitions">The input definitions the input system will use to map inputs and actions</param>
/// <param name="joinPolicy">The policy the input system uses for new users, devices, and the like</param>
public class InputSystemConfiguration(IEnumerable<IDeviceTopology> deviceTopologies, IEnumerable<InputConfiguration> supportedConfigurations, IEnumerable<ActionDefinition> definitions, InputSystemJoinPolicy joinPolicy)
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, IDeviceTopology> _topologyDescriptorLookup
        = deviceTopologies.Where(topology => topology is not null).ToDictionary(topology => topology.Name);
    private readonly Dictionary<string, InputConfiguration> _inputConfigurationLookup 
        = supportedConfigurations.Where(configuration => configuration is not null).ToDictionary(configuration => configuration.Id);
    private readonly Dictionary<string, ActionDefinition> _inputDefinitionLookup 
        = definitions?.Where(definition => definition?.Name is not null).ToDictionary(definition => definition.Name, StringComparer.OrdinalIgnoreCase) ?? [];

    #endregion

    #region Api

    /// <summary>
    /// The policy that determines new user, device, etc. behaviors when interacting with the input system
    /// </summary>
    public InputSystemJoinPolicy JoinPolicy => joinPolicy;

    public IReadOnlyList<IDeviceTopology> SupportedDeviceTopologies => [.. _topologyDescriptorLookup.Values];

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

    public IDeviceTopology? GetTopologyDescriptor(DeviceTopologyName topologyName)
        => _topologyDescriptorLookup.TryGetValue(topologyName, out var topology)
            ? topology
            : null;

    /// <summary>
    /// Attempts to get the definition from a name
    /// </summary>
    /// <param name="definitionName">The name of the definition to get</param>
    /// <returns>The definition if the name matches an existing definition in the input system, otherwise null</returns>
    public ActionDefinition? GetDefinition(string definitionName)
        => !string.IsNullOrWhiteSpace(definitionName) && _inputDefinitionLookup.TryGetValue(definitionName, out var definition)
            ? definition
            : null;

    #endregion
}
