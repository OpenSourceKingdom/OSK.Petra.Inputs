using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class DevicePointer(long pointerId, RuntimeDeviceIdentifier deviceIdentifier, long devicePointerId, PointerDetails details)
{
    public long PointerId => pointerId;

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    public long DevicePointerId => devicePointerId;

    public PointerDetails Details => details;

    internal DateTime Created { get; } = DateTime.Now; 
}
