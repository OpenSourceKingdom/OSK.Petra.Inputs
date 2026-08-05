using System;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public static class DeviceDescriptorExtensions
{
    public static bool IsGeneric(this IDeviceDescriptor descriptor)
        => descriptor.Identity.Name.Equals("Generic", StringComparison.OrdinalIgnoreCase);
}
