using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Models;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Internal.Services;

internal class DeviceDescriptorProvider(IEnumerable<IDeviceDescriptor> knownDescriptors, IInputConfigurationProvider configurationProvider) : IDeviceDescriptorProvider
{
    #region IDeviceDescriptorProvider

    public IDeviceDescriptor? GetDescriptorForDevice(DeviceIdentity deviceIdentity)
    {
        var deviceTopology = configurationProvider.Configuration.GetDeviceTopology(deviceIdentity.TopologyName);
        if (deviceTopology is null)
        {
            return null;
        }

        var descriptor = knownDescriptors.FirstOrDefault(d => d.DeviceIdentity == deviceIdentity) ?? knownDescriptors.FirstOrDefault(d => d.DeviceIdentity.TopologyName == deviceIdentity.TopologyName && d.DeviceIdentity.DeviceFamily == deviceIdentity.DeviceFamily);
        return descriptor is null
            ? new GenericDeviceDescriptor(deviceTopology, DeviceFamily.Generic)
            : descriptor;
    }

    public IEnumerable<IDeviceDescriptor> GetDescriptorsByToplogy(DeviceTopologyName topologyName)
    {
        var deviceTopology = configurationProvider.Configuration.GetDeviceTopology(topologyName);
        if (deviceTopology is null)
        {
            return [];
        }

        var descriptors = knownDescriptors.Where(descriptor => descriptor.DeviceIdentity.TopologyName == topologyName);

        return  descriptors.Append(new GenericDeviceDescriptor(deviceTopology, DeviceFamily.Generic));
    }

    #endregion
}
