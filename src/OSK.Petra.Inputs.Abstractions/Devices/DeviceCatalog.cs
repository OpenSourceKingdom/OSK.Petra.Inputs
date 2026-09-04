using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Contains all available device descriptors organized by topology.
/// </summary>
public class DeviceCatalog(IEnumerable<DevicePage> pages)
{
    #region Variables

    private readonly Dictionary<DeviceTopologyName, DevicePage> _pageLookup = pages.ToDictionary(part => part.TopologyName);

    #endregion

    #region Api

    /// <summary>
    /// Gets all device pages in the catalog.
    /// </summary>
    public IReadOnlyList<DevicePage> Pages => [.. _pageLookup.Values];

    /// <summary>
    /// Attempts to get the device page for the given topology name
    /// </summary>
    /// <param name="topologyName">The name of the topology</param>
    /// <returns>The device page, if it exists</returns>
    public DevicePage? GetPage(DeviceTopologyName topologyName)
        => _pageLookup.TryGetValue(topologyName, out var page)
            ? page
            : null;

    /// <summary>
    /// Retrieves a device descriptor by identity, falling back to generic device
    /// if the specific device not found.
    /// </summary>
    /// <returns>The device descriptor, if it exists</returns>
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
