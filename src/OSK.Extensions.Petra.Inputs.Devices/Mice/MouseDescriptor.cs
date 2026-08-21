using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Devices.Mice;

public abstract class MouseDescriptor: DeviceDescriptor<IMouseInput>
{
    #region Constructors

    public MouseDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    public MouseDescriptor(DeviceFamily family)
        : this(family, DeviceNames.Generic)
    {
    }

    public MouseDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Mouse, family, deviceName))
    {
    }

    #endregion
}
