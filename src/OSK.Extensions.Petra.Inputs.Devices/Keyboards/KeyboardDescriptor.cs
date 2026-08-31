using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Keyboards;

public abstract class KeyboardDescriptor: DeviceDescriptor<IKeyboardInput>
{
    #region Constructors

    public KeyboardDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    public KeyboardDescriptor(DeviceFamily family)
        : this(family, DeviceIdentities.GenericDeviceName)
    {
    }

    public KeyboardDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Keyboard, family, deviceName))
    {
    }

    #endregion
}
