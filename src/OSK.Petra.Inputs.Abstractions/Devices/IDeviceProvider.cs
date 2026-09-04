using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Provides devices for the input system to match against inputs being received from an application
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface IDeviceProvider
{
    /// <summary>
    /// Gets the devices the provider has access to
    /// </summary>
    /// <param name="cancellationToken">A cancellation token for the request</param>
    /// <returns>The devices the provider contains</returns>
    Task<Output<IEnumerable<IDeviceDescriptor>>> GetDevicesAsync(CancellationToken cancellationToken = default);
}
