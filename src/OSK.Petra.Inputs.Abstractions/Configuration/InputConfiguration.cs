using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents a set of device topologies that can be used together when interacting with the system.
/// For example, this could be a Keyboard or a Keyboard and Mouse
/// </summary>
public class InputConfiguration
{
    #region Static

    /// <summary>
    /// Gets a standard unique string id for a group of device topologies, using an enumeration of device identities
    /// </summary>
    /// <param name="deviceIdentities">The devices to get the id for</param>
    /// <returns>A standard unique string id for the group</returns>
    public static string GetConfigurationId(IEnumerable<DeviceIdentity> deviceIdentities)
        => GetConfigurationId(deviceIdentities.Select(identity => identity.TopologyName));

    /// <summary>
    /// Gets a standard unique string id for a group of device topologies, using a parameter collection of device identities
    /// </summary>
    /// <param name="deviceIdentities">The devices to get the id for</param>
    /// <returns>A standard unique string id for the group</returns>
    public static string GetConfigurationId(params DeviceIdentity[] deviceIdentities)
        => GetConfigurationId(deviceIdentities.Select(identity => identity.TopologyName));

    /// <summary>
    /// Gets a standard unique string id for a group of device topologies
    /// </summary>
    /// <param name="topologies">The device topologies in the group</param>
    /// <returns>A standard unique string id for the group</returns>
    public static string GetConfigurationId(params DeviceTopologyName[] topologies)
        => GetConfigurationId((IEnumerable<DeviceTopologyName>)topologies);

    /// <summary>
    /// Gets a standard unique string id for a group of device topologies, using a parameter collection of device topologies
    /// </summary>
    /// <param name="topologies">The device topologies in the group</param>
    /// <returns>A standard unique string id for the group</returns>
    public static string GetConfigurationId(IEnumerable<DeviceTopologyName> topologies)
        // Sequences that are the same but out of order must match - i.e. Keyboard + Mouse == Mouse + Keyboard
        => string.Join(".", topologies.Distinct().Select(topology => topology.Name).OrderBy(name => name));

    #endregion

    #region Variables

    /// <summary>
    /// Scheme Lookup keys:
    /// - Definition Name
    /// - Scheme Name
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, InputScheme>> _inputSchemeLookup = new(StringComparer.OrdinalIgnoreCase);

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an input configuration with the given topologies
    /// </summary>
    /// <param name="topologyNames">The names of the device topologies the configuration will suppport</param>
    /// <exception cref="ArgumentNullException">If the topology enumeration is null</exception>
    public InputConfiguration(IEnumerable<DeviceTopologyName> topologyNames)
    {
        TopologyNames = topologyNames is null ? throw new ArgumentNullException(nameof(topologyNames)) : [.. topologyNames];
        Id = GetConfigurationId(TopologyNames);
    }

    #endregion

    #region Api

    /// <summary>
    /// A unique id that is based on the device topologies this configuration refers to
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// A read-only enumeration for the device topologies that the configuration supports
    /// </summary>
    public IReadOnlyList<DeviceTopologyName> TopologyNames { get; }

    /// <summary>
    /// The collection of devices the group refers to
    /// </summary>
    public IReadOnlyList<InputScheme> Schemes => [.. _inputSchemeLookup.Values.SelectMany(schemeLookup => schemeLookup.Values)];

    /// <summary>
    /// Attempts to get the input scheme associated with the gien <see cref="ActionDefinition"/> name and scheme name
    /// </summary>
    /// <param name="definitionName">The action definition name</param>
    /// <param name="schemeName">The name of the scheme</param>
    /// <returns>The Input Scheme, if it exists</returns>
    public InputScheme? GetScheme(string definitionName, string schemeName)
        => _inputSchemeLookup.TryGetValue(definitionName, out var schemeLookup)
            ? schemeLookup.TryGetValue(schemeName, out var scheme) ? scheme : null
            : null;

    /// <summary>
    /// Calculates a device support confidence score based on the topology provided. The output of this function can be used to order lists of supported 
    /// input configurations to get the first 'strongest' configuration that matches the family. If there are multiple configurations that provide support to a given
    /// device identity, the score is determined then by how many devices the configuration needs. For example, a keyboard only configuration should match a keyboard
    /// family at 1 whereas a keyboard + mouse configuration should match at .5
    /// </summary>
    /// <param name="deviceIdentity">The type to get a support confidence for</param>
    /// <returns>A score between 0 and 1 that represents the confidence level this input configuration will support a given device family</returns>
    public float GetDeviceSupportConfidence(DeviceIdentity deviceIdentity)
    {
        if (Schemes.Count is 0)
        {
            return 0;
        }
        if (!TopologyNames.Contains(deviceIdentity.TopologyName))
        {
            return 0;
        }
        if (Schemes.Any(scheme => scheme.ContainsDevice(deviceIdentity)))
        {
            return 1;
        }
        if (Schemes.Any(scheme => scheme.ContainsFamily(deviceIdentity.DeviceFamily)))
        {
            return 0.75f;
        }

        // If there is a generic scheme, we can assume that it will support the device family at a lower confidence level than a specific scheme,
        // but a configuration that contains no generic should be considered a higher confidence than configurations that don't match the topology at all,
        return Schemes.Any(scheme => scheme.ContainsFamily(DeviceFamily.Generic))
            ? .5f
            : .1f;
    }

    /// <summary>
    /// Determines if the provided device identity is in the group
    /// </summary>
    /// <param name="identity">The identity to check the configuration for</param>
    /// <returns>Whether this configuration includes the device in question</returns>
    public bool Contains(DeviceIdentity identity) 
        => TopologyNames.Contains(identity.TopologyName);

    /// <summary>
    /// Attempts to create a display name for the topologies that is more readable: "Keyboard", "Keyboard and Mouse", etc.
    /// </summary>
    /// <returns>A displayable text name for the configuration</returns>
    public string GetDisplayName()
    {
        return TopologyNames.Count switch
        {
            0 => string.Empty,
            1 =>$"{TopologyNames[0].Name}",
            2 => $"{TopologyNames[0].Name} and {TopologyNames[1].Name}",
            _ => $"{string.Join(", ", TopologyNames.Take(TopologyNames.Count - 1).Select(device => device.Name))}, and {TopologyNames[^1].Name}"
        };
    }

    /// <summary>
    /// Adds an input scheme to the configuration
    /// </summary>
    /// <param name="scheme">The scheme to include in the configuration</param>
    /// <exception cref="ArgumentNullException">If the scheme is null</exception>
    /// <exception cref="InvalidOperationException">If validation fails for the scheme</exception>
    public void AddScheme(InputScheme scheme)
    {
        if (scheme is null)
        {
            throw new ArgumentNullException(nameof(scheme), "Scheme can not be null");
        }
        if (string.IsNullOrWhiteSpace(scheme.Name))
        {
            throw new InvalidOperationException("Scheme name can not be empty.");
        }
        if (string.IsNullOrWhiteSpace(scheme.DefinitionName))
        {
            throw new InvalidOperationException("Scheme definition name can not be empty.");
        }

        if (!_inputSchemeLookup.TryGetValue(scheme.DefinitionName, out var definitionSchemeLookup))
        {
            definitionSchemeLookup = new(StringComparer.OrdinalIgnoreCase);
            _inputSchemeLookup[scheme.DefinitionName] = definitionSchemeLookup;
        }
        if (definitionSchemeLookup.TryGetValue(scheme.Name, out _))
        {
            throw new InvalidOperationException($"Unable to add scheme '{scheme.Name}' to the definition '{scheme.DefinitionName}' since a scheme with the same name already exists.");
        }

        definitionSchemeLookup[scheme.Name] = scheme;
    }

    #endregion
}
