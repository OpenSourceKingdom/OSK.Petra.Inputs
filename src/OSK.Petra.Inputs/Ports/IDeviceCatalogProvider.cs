using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Ports;

public interface IDeviceCatalogProvider
{
    Task<Output> InitializeAsync(CancellationToken cancellationToken = default);

    DeviceCatalog GetCatalog(DeviceTopologyName topologyName);
}
