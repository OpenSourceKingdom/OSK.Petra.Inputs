using System;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public static class DeviceDescriptorExtensions
{
    extension(IDeviceDescriptor descriptor)
    {
        public bool IsGeneric() 
            => descriptor.Identity.Name.Equals(DeviceNames.Generic, StringComparison.OrdinalIgnoreCase);
    }
}
