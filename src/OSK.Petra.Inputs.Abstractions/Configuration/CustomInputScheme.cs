using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents a custom scheme for an input definition
/// </summary>
public class CustomInputScheme
{
    #region Variables

    /// <summary>
    /// The <see cref="ActionDefinition"/> this scheme is associated with 
    /// </summary>
    public required string DefinitionName { get; init; }

    /// <summary>
    /// The unique name for the scheme on the definition
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The collection of device maps for this scheme
    /// </summary>
    public required List<DeviceInputMap> DeviceMaps { get; init; }

    #endregion

    #region Helpers

    public IEnumerable<DeviceIdentity> GetDeviceIdentities()
        => DeviceMaps.Select(map => map.DeviceIdentity);

    public InputScheme ToInputScheme()
        => new(DefinitionName, Name, DeviceMaps, isDefault: false, isCustom: true);

    #endregion
}
