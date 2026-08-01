using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public class InputScheme(string definitionName, string name, IEnumerable<DeviceInputMap> deviceMaps, bool isDefault, bool isCustom)
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, DeviceInputMap> _deviceMapLookup
        = deviceMaps?.Where(map => map is not null).ToDictionary(map => map.DeviceIdentity.TopologyName) ?? [];

    #endregion

    #region Api

    public string DefinitionName => definitionName;

    /// <summary>
    /// A unique name for the scheme
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Whether the scheme was created by a user
    /// </summary>
    public bool IsCustom => isCustom;

    /// <summary>
    /// Whether this scheme should be used before others, if no scheme has been selected by a user
    /// </summary>
    public bool IsDefault => isDefault;

    /// <summary>
    /// The collection of device input maps supported by this scheme
    /// </summary>
    public IReadOnlyCollection<DeviceInputMap> DeviceMaps => _deviceMapLookup.Values;

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
    /// Gets the device types this scheme uses
    /// </summary>
    /// <returns>The collection of device types</returns>
    public IEnumerable<DeviceTopologyName> GetDeviceTypes()
        => _deviceMapLookup.Values.Select(map => map.DeviceIdentity.TopologyName);

    #endregion
}
