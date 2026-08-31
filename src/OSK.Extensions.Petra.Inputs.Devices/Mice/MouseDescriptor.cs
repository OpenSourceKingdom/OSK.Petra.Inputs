using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Mice;

public abstract class MouseDescriptor: DeviceDescriptor<IMouseInput>
{
    #region Constructors

    public MouseDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    public MouseDescriptor(DeviceFamily family)
        : this(family, DeviceIdentities.GenericDeviceName)
    {
    }

    public MouseDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Mouse, family, deviceName))
    {
    }

    #endregion
}
