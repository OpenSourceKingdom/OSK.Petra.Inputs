using System;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public static class DeviceDescriptorExtensions
{
    extension(IDeviceDescriptor descriptor)
    {
        /// <summary>
        /// Validates if the current device is in a generic family
        /// </summary>
        /// <returns>Whether the family is generic</returns>
        public bool IsGenericFamily()
            => descriptor.Identity.DeviceFamily == DeviceFamily.Generic;

        /// <summary>
        /// Validates if the current device is generic
        /// </summary>
        /// <returns>Whether the device is generic</returns>
        public bool IsGenericDevice() 
            => descriptor.Identity.Name.Equals(DeviceIdentities.GenericDeviceName, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Validates if the current device is a complete generic. e.g. this is the combination of Generic family + device
        /// </summary>
        /// <returns>Whether the device is a complete generic</returns>
        public bool IsGeneric()
            => descriptor.IsGenericFamily() && descriptor.IsGenericDevice();
    }
}
