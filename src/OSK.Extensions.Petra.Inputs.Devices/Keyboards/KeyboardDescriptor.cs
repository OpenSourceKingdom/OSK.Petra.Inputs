using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Devices.Keyboards;

public abstract class KeyboardDescriptor: DeviceDescriptor<IKeyboardInput>
{
    #region Constructors

    public KeyboardDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    public KeyboardDescriptor(DeviceFamily family)
        : this(family, DeviceNames.Generic)
    {
    }

    public KeyboardDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Keyboard, family, deviceName))
    {
    }

    #endregion
}
