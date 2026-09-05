using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// Represents a pointer on a specific device
/// </summary>
/// <param name="pointerId">The id of the pointer</param>
/// <param name="deviceIdentifier">The device the pointer is originating from</param>
/// <param name="devicePointerId">The device's id for the pointer</param>
/// <param name="details">The detail information for the particular pointer</param>
public class DevicePointer(long pointerId, RuntimeDeviceIdentifier deviceIdentifier, long devicePointerId, PointerDetails details)
{
    /// <summary>
    /// The pointer id within the input system
    /// </summary>
    public long PointerId => pointerId;

    /// <summary>
    /// The device the pointer is originating from
    /// </summary>
    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    /// <summary>
    /// The device specific id for the pointer 
    /// </summary>
    public long DevicePointerId => devicePointerId;

    /// <summary>
    /// Details that describe the pointer
    /// </summary>
    public PointerDetails Details => details;

    internal DateTime Created { get; } = DateTime.Now; 
}
