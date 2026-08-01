using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Internal;

internal interface IDeviceDescriptorProvider
{
    IDeviceDescriptor? GetDescriptorForDevice(DeviceIdentity deviceIdentity);

    IEnumerable<IDeviceDescriptor> GetDescriptorsByToplogy(DeviceTopologyName topologyName);
}
