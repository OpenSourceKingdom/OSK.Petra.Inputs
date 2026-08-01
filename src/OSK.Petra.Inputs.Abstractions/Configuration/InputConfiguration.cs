using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents a set of device topologies that can be used together when interacting with the system.
/// For example, this could be a Keyboard or a Keyboard and Mouse
/// </summary>
public class InputConfiguration
{
    #region Static

    public static string GetConfigurationId(IEnumerable<DeviceIdentity> deviceIdentities)
        => GetConfigurationId(deviceIdentities.Select(identity => identity.TopologyName));

    /// <summary>
    /// Gets a standard unique string id for a group of device topologies
    /// </summary>
    /// <param name="topologies">The device topologies in the group</param>
    /// <returns>A standard unique string id for the group</returns>
    public static string GetConfigurationId(IEnumerable<DeviceTopologyName> topologies)
        // Sequences that are the same but out of order must match - i.e. Keyboard + Mouse == Mouse + Keyboard
        => string.Join(".", topologies.Distinct().OrderBy(name => name));

    #endregion

    #region Variables

    private readonly Dictionary<string, Dictionary<string, InputScheme>> _inputSchemeLookup = [];

    #endregion

    #region Constructors

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

    public IReadOnlyList<DeviceTopologyName> TopologyNames { get; }

    /// <summary>
    /// The collection of devices the group refers to
    /// </summary>
    public IReadOnlyList<InputScheme> Schemes => [.. _inputSchemeLookup.Values.SelectMany(schemeLookup => schemeLookup.Values)];

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

        return TopologyNames.Contains(deviceIdentity.TopologyName)
            ? TopologyNames.Count is 1 ? 1 : 1 / TopologyNames.Count
            : 0;
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
            1 =>$"{TopologyNames[0]}",
            2 => $"{TopologyNames[0]} and {TopologyNames[1]}",
            _ => $"{string.Join(", ", TopologyNames.Take(TopologyNames.Count - 1).Select(device => device))}, and {TopologyNames[^1]}"
        };
    }

    #endregion
}
