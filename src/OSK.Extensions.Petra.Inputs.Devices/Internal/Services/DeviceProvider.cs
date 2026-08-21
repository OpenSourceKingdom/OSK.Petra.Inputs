using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Extensions.Petra.Inputs.Devices.Internal.Services;

internal class DeviceProvider : IDeviceProvider
{
    #region IDeviceProvider

    public Task<Output<IEnumerable<IDeviceDescriptor>>> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
