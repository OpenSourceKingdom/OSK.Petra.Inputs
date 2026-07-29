using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// The very necessary configuration utilized with the input system. This is the 'source of truth' for all interactions 
/// and decisions made by the input system
/// </summary>
/// <param name="supportedConfigurations">The device topologies the input system is able to support</param>
/// <param name="definitions">The input definitions the input system will use to map inputs and actions</param>
/// <param name="processorConfiguration">The configuration for the internal input processing</param>
/// <param name="joinPolicy">The policy the input system uses for new users, devices, and the like</param>
public class InputSystemConfiguration(IEnumerable<IDeviceTopology> deviceTopologies, IEnumerable<InputConfiguration> supportedConfigurations, IEnumerable<ActionDefinition> definitions, InputSystemJoinPolicy joinPolicy)
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, IDeviceTopology> _deviceTopologyLookup
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

    public IReadOnlyList<IDeviceTopology> SupportedDeviceTopologies => [.. _deviceTopologyLookup.Values];

    /// <summary>
    /// The collection of supported input device combinations for built in and custom schemes to use.
    /// </summary>
    public IReadOnlyList<InputConfiguration> InputConfigurations 
        => [.. _inputConfigurationLookup.Values];

    /// <summary>
    /// The collection of input definitions that are available for users
    /// </summary>
    public IReadOnlyList<ActionDefinition> Definitions
        => [.. _inputDefinitionLookup.Values];

    /// <summary>
    /// Attempts to get an input configuration by the device identity
    /// </summary>
    /// <param name="deviceIdentity">The identity of the device to get a topology for</param>
    /// <returns>The specific topology for the device identity if it is supported, otherwise null</returns>
    public InputConfiguration? GetBestInputConfiguration(DeviceIdentity deviceIdentity)
        => InputConfigurations.OrderByDescending(configuration => configuration.GetDeviceSupportConfidence(deviceIdentity)).FirstOrDefault();

    public InputConfiguration? GetInputConfiguration(string configurationId)
        => _inputConfigurationLookup.TryGetValue(configurationId, out var configuration)
            ? configuration
            : null;

    public IDeviceTopology? GetDeviceTopology(DeviceIdentity deviceIdentity)
        => _deviceTopologyLookup.TryGetValue(deviceIdentity.TopologyName, out var topology)
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
