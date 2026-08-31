using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Internal.Services;

internal class DeviceCatalogProvider(IEnumerable<IDeviceProvider> deviceProviders) : IDeviceCatalogProvider
{
    #region Variables

    private DeviceCatalog? _catalog;

    #endregion

    #region IDeviceCatalogProvider

    public Task<Output<DeviceCatalog>> GetCatalogAsync(CancellationToken cancellationToken = default)
        => LoadCatalogAsync(cancellationToken);

    #endregion

    #region Helpers

    private async Task<Output<DeviceCatalog>> LoadCatalogAsync(CancellationToken cancellationToken = default)
    {
        if (_catalog is not null)
        {
            return Out.Success(_catalog);
        }

        var allDevices = new List<IDeviceDescriptor>();

        foreach (var deviceProvider in deviceProviders)
        {
            var getDevicesOutput = await deviceProvider.GetDevicesAsync(cancellationToken);
            if (!getDevicesOutput.IsSuccessful)
            {
                return getDevicesOutput.As<DeviceCatalog>();
            }

            allDevices.AddRange(getDevicesOutput.Data);
        }

        var pages = allDevices.GroupBy(group => group.Identity.TopologyName)
                              .Select(topologyGroup => new { TopologyName = topologyGroup.Key, Devices = topologyGroup.GroupBy(descriptor => descriptor.Identity).Select(identityGroups => identityGroups.First()) })
                              .Select(topologyGroup => new DevicePage(topologyGroup.TopologyName, topologyGroup.Devices));

        _catalog = new DeviceCatalog(pages);

        return Out.Success(_catalog);
    }

    #endregion
}
