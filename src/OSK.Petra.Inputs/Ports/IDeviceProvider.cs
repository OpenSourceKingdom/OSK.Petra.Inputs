using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Ports;

public interface IDeviceProvider
{
    Task<Output<IEnumerable<IDeviceDescriptor>>> GetDevicesAsync(CancellationToken cancellationToken = default);
}
