using OSK.Operations.Outputs.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OSK.Petra.Inputs.Abstractions.Devices;

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
