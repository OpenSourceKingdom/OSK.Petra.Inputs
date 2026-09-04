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

    public InputScheme(string definitionName, string name, IEnumerable<DeviceInputMap> deviceMaps, bool isDefault, bool isCustom)
        : this(definitionName, name, deviceMaps, [], isDefault, isCustom)
    {

    }

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

    public DeviceInputActionMap? GetDeviceInputMap(DeviceIdentity deviceIdentity, long inputId)
        => GetDeviceMap(deviceIdentity)?.GetInputMap(inputId);

    public VirtualInputActionMap? GetVirtualInputMap(IVirtualInput virtualInput)
        => _virtualMapLookup.TryGetValue(virtualInput, out var map)
            ? map
            : null;

    public IEnumerable<DeviceIdentity> GetDeviceIdentities()
        => DeviceMaps.Select(map => map.DeviceIdentity);

    public bool ContainsTopology(DeviceTopologyName topologyName)
        => _deviceMapLookup.ContainsKey(topologyName);

    public bool ContainsFamily(DeviceFamily family)
        => _deviceMapLookup.Values.Any(map => map.DeviceIdentity.DeviceFamily == family);

    public bool ContainsDevice(DeviceIdentity deviceIdentity)
        => _deviceMapLookup.Values.Any(map => map.DeviceIdentity == deviceIdentity);

    #endregion
}
