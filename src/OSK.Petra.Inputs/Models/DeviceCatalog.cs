using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Models;

public class DeviceCatalog(IEnumerable<DevicePage> pages)
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, DevicePage> _pageLookup = pages.ToDictionary(part => part.TopologyName);

    #endregion

    #region Api


    public IReadOnlyList<DevicePage> Pages => [.. _pageLookup.Values];

    public DevicePage? GetPage(DeviceTopologyName topologyName)
        => _pageLookup.TryGetValue(topologyName, out var part)
            ? part
            : null;

    #endregion
}
