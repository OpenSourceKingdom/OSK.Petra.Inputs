using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public class InputScheme
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, DeviceInputMap> _deviceMapLookup;
    private readonly Dictionary<IVirtualInput, VirtualInputActionMap> _virtualMapLookup;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an input scheme, without virtual inputs
    /// </summary>
    /// <param name="definitionName">The action definition this scheme refers to</param>
    /// <param name="name">The unique name of the scheme</param>
    /// <param name="deviceMaps">The coollection of device input maps the scheme is configured to use</param>
    /// <param name="isDefault">Whether this scheme is the default</param>
    /// <param name="isCustom">Whether this scheme is a custom scheme, created by a user</param>
    public InputScheme(string definitionName, string name, IEnumerable<DeviceInputMap> deviceMaps, bool isDefault, bool isCustom)
        : this(definitionName, name, deviceMaps, [], isDefault, isCustom)
    {

    }

    /// <summary>
    /// Creates an input scheme, with virtual inputs
    /// </summary>
    /// <param name="definitionName">The action definition this scheme refers to</param>
    /// <param name="name">The unique name of the scheme</param>
    /// <param name="deviceMaps">The coollection of device input maps the scheme is configured to use</param>
    /// <param name="virtualMaps">The collection of virtual maps the scheme is configured to use</param>
    /// <param name="isDefault">Whether this scheme is the default</param>
    /// <param name="isCustom">Whether this scheme is a custom scheme, created by a user</param>
    public InputScheme(string definitionName, string name, IEnumerable<DeviceInputMap> deviceMaps, IEnumerable<VirtualInputActionMap> virtualMaps, bool isDefault, bool isCustom)
    {
        DefinitionName = definitionName;
        Name = name;
        IsDefault = isDefault;
        IsCustom = isCustom;

        _deviceMapLookup = deviceMaps?.Where(map => map is not null).ToDictionary(map => map.DeviceIdentity.TopologyName) ?? [];
        _virtualMapLookup = virtualMaps?.ToDictionary(map => map.VirtualInput) ?? [];
    }

    #endregion

    #region Api

    /// <summary>
    /// The action definition this scheme refers to
    /// </summary>
    public string DefinitionName { get; }

    /// <summary>
    /// A unique name for the scheme
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Whether this scheme should be used before others, if no scheme has been selected by a user
    /// </summary>
    public bool IsDefault { get; }

    /// <summary>
    /// Whether the scheme was created by a user
    /// </summary>
    public bool IsCustom { get; }

    /// <summary>
    /// The collection of device input maps supported by this scheme
    /// </summary>
    public IReadOnlyCollection<DeviceInputMap> DeviceMaps => [.. _deviceMapLookup.Values];

    /// <summary>
    /// The collection of virtual input maps supported by this scheme
    /// </summary>
    public IReadOnlyCollection<VirtualInputActionMap> VirtualMaps => [.. _virtualMapLookup.Values];

    /// <summary>
    /// Attempts to get a device map for a device identity
    /// </summary>
    /// <param name="deviceIdentity">The identity for a device to get maps for</param>
    /// <returns>The device map if one is configured, otherwise null</returns>
    public DeviceInputMap? GetDeviceMap(DeviceIdentity deviceIdentity)
        => _deviceMapLookup.TryGetValue(deviceIdentity.TopologyName, out var map)
            ? map
            : null;

    /// <summary>
    /// Attempts to get an action mapping for the provided input id
    /// </summary>
    /// <param name="deviceIdentity">The device the input is associated with</param>
    /// <param name="inputId">The id of the input to get the map for</param>
    /// <returns>The action map for the input, if it exists</returns>
    public DeviceInputActionMap? GetDeviceInputMap(DeviceIdentity deviceIdentity, long inputId)
        => GetDeviceMap(deviceIdentity)?.GetInputMap(inputId);

    /// <summary>
    /// Attempts to get an action mapping for the provided virtual input
    /// </summary>
    /// <param name="virtualInput">The input to get the map for</param>
    /// <returns>The action map for the input, if it exists</returns>
    public VirtualInputActionMap? GetVirtualInputMap(IVirtualInput virtualInput)
        => _virtualMapLookup.TryGetValue(virtualInput, out var map)
            ? map
            : null;

    /// <summary>
    /// Gets the device identities associated with this custom scheme
    /// </summary>
    /// <returns>The device identities</returns>
    public IEnumerable<DeviceIdentity> GetDeviceIdentities()
        => DeviceMaps.Select(map => map.DeviceIdentity);

    /// <summary>
    /// Checks whether the provided topology name is included in the scheme mappings
    /// </summary>
    /// <param name="topologyName">The toplogy to check for</param>
    /// <returns>Whether the scheme has the provided topology</returns>
    public bool ContainsTopology(DeviceTopologyName topologyName)
        => _deviceMapLookup.ContainsKey(topologyName);

    /// <summary>
    /// Checks whether the input scheme includes the specified family
    /// </summary>
    /// <param name="family">The family to check for</param>
    /// <returns>Whether the scheme has the provided family</returns>
    public bool ContainsFamily(DeviceFamily family)
        => _deviceMapLookup.Values.Any(map => map.DeviceIdentity.DeviceFamily == family);

    /// <summary>
    /// Checks whether the input scheme includes the specified device
    /// </summary>
    /// <param name="deviceIdentity">The device to check for</param>
    /// <returns>Whether the scheme has the provided device</returns>
    public bool ContainsDevice(DeviceIdentity deviceIdentity)
        => _deviceMapLookup.Values.Any(map => map.DeviceIdentity == deviceIdentity);

    #endregion
}
