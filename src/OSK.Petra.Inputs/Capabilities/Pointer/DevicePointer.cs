using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class DevicePointer(int pointerId, RuntimeDeviceIdentifier deviceIdentifier, int devicePointerId, PointerDetails details)
{
    public int PointerId => pointerId;

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    public int DevicePointerId => devicePointerId;

    public PointerDetails Details => details;

    internal DateTime Created { get; } = DateTime.Now; 
}
