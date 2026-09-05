using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Mice;

/// <summary>
/// Represents a mouse device
/// </summary>
public abstract class MouseDescriptor: DeviceDescriptor<IMouseInput>
{
    #region Constructors

    /// <summary>
    /// Create a generic mouse
    /// </summary>
    public MouseDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    /// <summary>
    /// Creates a generic mouse for the famly
    /// </summary>
    /// <param name="family">The device family</param>
    public MouseDescriptor(DeviceFamily family)
        : this(family, DeviceIdentities.GenericDeviceName)
    {
    }

    /// <summary>
    /// Creates a mouse
    /// </summary>
    /// <param name="family">The family for the mouse</param>
    /// <param name="deviceName">The device name</param>
    public MouseDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Mouse, family, deviceName))
    {
    }

    #endregion
}
