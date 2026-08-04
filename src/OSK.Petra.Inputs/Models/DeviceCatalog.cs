using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Models;

public class DeviceCatalog(IEnumerable<DeviceCatalogPart> parts)
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, DeviceCatalogPart> _partLookup = parts.ToDictionary(part => part.TopologyName);

    #endregion

    #region Api


    public IReadOnlyList<DeviceCatalogPart> Parts => [.. _partLookup.Values];

    public DeviceCatalogPart? GetPart(DeviceTopologyName topologyName)
        => _partLookup.TryGetValue(topologyName, out var part)
            ? part
            : null;

    #endregion
}
