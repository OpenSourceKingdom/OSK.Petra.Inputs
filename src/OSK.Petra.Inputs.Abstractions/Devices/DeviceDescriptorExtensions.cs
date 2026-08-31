using System;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public static class DeviceDescriptorExtensions
{
    extension(IDeviceDescriptor descriptor)
    {
        public bool IsGeneric() 
            => descriptor.Identity.Name.Equals(DeviceIdentities.GenericDeviceName, StringComparison.OrdinalIgnoreCase);
    }
}
