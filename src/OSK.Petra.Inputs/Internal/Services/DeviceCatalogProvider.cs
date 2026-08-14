using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Internal.Services;

internal class DeviceCatalogProvider(IInputSystemConfigurationProvider configurationProvider, IEnumerable<IDeviceProvider> deviceProviders) : IDeviceCatalogProvider
{
    #region Variables

    private DeviceCatalog? _catalog;

    #endregion

    #region IDeviceCatalogProvider

    public async Task<Output<DeviceCatalog>> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        if (_catalog is null)
        {
            var loadCatalogOutput = await LoadCatalogAsync(cancellationToken);
            if (!loadCatalogOutput.IsSuccessful)
            {
                return loadCatalogOutput;
            }

            _catalog = loadCatalogOutput.Data;
        }

        return Out.Success(_catalog);
    }

    #endregion

    #region Helpers

    private async Task<Output<DeviceCatalog>> LoadCatalogAsync(CancellationToken cancellationToken = default)
    {
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

        var partDeviceLookup = allDevices.GroupBy(group => group.Identity.TopologyName)
                                         .Select(topologyGroup => new { TopologyName = topologyGroup.Key, Devices = topologyGroup.GroupBy(descriptor => descriptor.Identity).Select(identityGroups => identityGroups.First()) })
                                         .ToDictionary(group => group.TopologyName, group => group.Devices);

        var catalogParts = configurationProvider.Configuration.SupportedDeviceTopologies.Select(toplogyDefinition => new DeviceCatalogPart()
        {
            TopologyName = toplogyDefinition.Name,
            GenericDevice = toplogyDefinition.CreateGeneric(),
            KnownDevices = partDeviceLookup.TryGetValue(toplogyDefinition.Name, out var knownDevices)
                ? [.. knownDevices]
                : []
        });

        return Out.Success(new DeviceCatalog(catalogParts));
    }

    #endregion
}
