using System;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public static class DeviceDescriptorExtensions
{
    extension(IDeviceDescriptor descriptor)
    {
        public bool IsGenericFamily()
            => descriptor.Identity.DeviceFamily == DeviceFamily.Generic;

        public bool IsGenericDevice() 
            => descriptor.Identity.Name.Equals(DeviceIdentities.GenericDeviceName, StringComparison.OrdinalIgnoreCase);

        public bool IsGeneric()
            => descriptor.IsGenericFamily() && descriptor.IsGenericDevice();
    }
}
