using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public class DeviceCatalog(IEnumerable<DevicePage> pages)
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, DevicePage> _pageLookup = pages.ToDictionary(part => part.TopologyName);

    #endregion

    #region Api


    public IReadOnlyList<DevicePage> Pages => [.. _pageLookup.Values];

    public DevicePage? GetPage(DeviceTopologyName topologyName)
        => _pageLookup.TryGetValue(topologyName, out var page)
            ? page
            : null;

    public IDeviceDescriptor? GetDevice(DeviceIdentity deviceIdentity)
    {
        if (!_pageLookup.TryGetValue(deviceIdentity.TopologyName, out var page))
        {
            return null;
        }

        var matchedDevice = page.Devices.FirstOrDefault(device => device.Identity == deviceIdentity) 
            ?? page.Devices.FirstOrDefault(device => device.Identity.DeviceFamily == deviceIdentity.DeviceFamily && device.IsGenericDevice());

        return matchedDevice ?? page.GenericDevice;
    }

    #endregion
}
