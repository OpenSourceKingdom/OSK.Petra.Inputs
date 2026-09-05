using OSK.Petra.Inputs.Abstractions.Devices;
using System.Collections.Generic;

namespace OSK.Extensions.Petra.Inputs.Devices.Mice;

/// <summary>
/// Represents a mouse device
/// </summary>
public class MouseDescriptor: DeviceDescriptor<IMouseInput>
{
    #region Constructors

    /// <summary>
    /// Create a generic mouse
    /// </summary>
    /// <param name="mouseInputs">The available mouse inputs</param>
    public MouseDescriptor(IEnumerable<IMouseInput> mouseInputs)
        : this(DeviceFamily.Generic, mouseInputs)
    {
    }

    /// <summary>
    /// Creates a generic mouse for the famly
    /// </summary>
    /// <param name="family">The device family</param>
    /// <param name="mouseInputs">The available mouse inputs</param>
    public MouseDescriptor(DeviceFamily family, IEnumerable<IMouseInput> mouseInputs)
        : this(family, DeviceIdentities.GenericDeviceName, mouseInputs)
    {
    }

    /// <summary>
    /// Creates a mouse
    /// </summary>
    /// <param name="family">The family for the mouse</param>
    /// <param name="deviceName">The device name</param>
    /// <param name="mouseInputs">The available mouse inputs</param>
    public MouseDescriptor(DeviceFamily family, string deviceName, IEnumerable<IMouseInput> mouseInputs)
        : base(new(DeviceTopologyName.Mouse, family, deviceName), mouseInputs)
    {
    }

    #endregion
}
