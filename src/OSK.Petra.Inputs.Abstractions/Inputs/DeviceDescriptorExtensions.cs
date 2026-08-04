using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public static class DeviceDescriptorExtensions
{
    public static bool IsGeneric(this IDeviceDescriptor descriptor)
        => descriptor.Identity.Name.Equals("Generic", StringComparison.OrdinalIgnoreCase);
}
