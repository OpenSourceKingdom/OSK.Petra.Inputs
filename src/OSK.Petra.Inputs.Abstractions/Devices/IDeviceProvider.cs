using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Abstractions.Devices;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IDeviceProvider
{
    Task<Output<IEnumerable<IDeviceDescriptor>>> GetDevicesAsync(CancellationToken cancellationToken = default);
}
