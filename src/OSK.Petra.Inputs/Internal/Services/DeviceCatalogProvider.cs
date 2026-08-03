using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Internal.Services;

internal class DeviceCatalogProvider(IInputSystemConfigurationProvider configurationProvider, IEnumerable<IDeviceProvider> deviceProviders) : IDeviceCatalogProvider
{
    #region Variables

    private bool _initialized;
    private readonly Dictionary<DeviceTopologyName, IReadOnlyList<IDeviceDescriptor>> _deviceDescriptorLookup = [];

    #endregion

    #region IDeviceCatalogProvider

    public DeviceCatalog GetCatalog(DeviceTopologyName topologyName)
    {
        var topologyDescriptor = configurationProvider.Configuration.GetTopologyDescriptor(topologyName);
        if (topologyDescriptor is null)
        {
            // Topology isn't supported, so no devices to return
            return new DeviceCatalog()
            {
                TopologyName = topologyName,
                GenericDevice = null,
                KnownDevices = []
            };
        }

        return new DeviceCatalog()
        {
            TopologyName = topologyName,
            GenericDevice = topologyDescriptor.CreateGeneric(),
            KnownDevices = _deviceDescriptorLookup.TryGetValue(topologyName, out var knownDescriptors)
                ? [.. knownDescriptors]
                : []
        };
    }

    public async Task<Output> InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
        {
            return Out.Success();
        }

        _deviceDescriptorLookup.Clear();
        var allDevices = new List<IDeviceDescriptor>();

        foreach (var deviceProvider in deviceProviders)
        {
            var getDevicesOutput = await deviceProvider.GetDevicesAsync(cancellationToken);
            if (!getDevicesOutput.IsSuccessful)
            {
                return getDevicesOutput;
            }

            allDevices.AddRange(getDevicesOutput.Data);
        }

        foreach (var deviceGroup in allDevices.GroupBy(device => device.Identity.TopologyName))
        {
            _deviceDescriptorLookup[deviceGroup.Key] = [.. deviceGroup.GroupBy(group => group.Identity).Select(identityGroup => identityGroup.First())];
        }

        _initialized = true;
        return Out.Success();
    }

    #endregion
}
