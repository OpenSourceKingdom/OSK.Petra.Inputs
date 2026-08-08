using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Ports;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IDeviceCatalogProvider
{
    Task<Output<DeviceCatalog>> GetCatalogAsync(CancellationToken cancellationToken = default);
}
