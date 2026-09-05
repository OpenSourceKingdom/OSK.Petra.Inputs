using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// A provider that returns detailed device information on a per topology basis
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IDeviceCatalogProvider
{
    /// <summary>
    /// Retrieves the device catalog containing all available device information.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>
    /// An output containing the device catalog if the retrieval succeeded
    /// </returns>
    Task<Output<DeviceCatalog>> GetCatalogAsync(CancellationToken cancellationToken = default);
}
