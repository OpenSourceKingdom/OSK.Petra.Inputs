using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Devices.Gamepads;

public abstract class GamepadDescriptor: DeviceDescriptor<IGamepadInput>
{
    #region Constructors

    public GamepadDescriptor()
        : this(DeviceFamily.Generic)
    {
    }

    public GamepadDescriptor(DeviceFamily family)
        : this(family, DeviceNames.Generic)
    {
    }

    public GamepadDescriptor(DeviceFamily family, string deviceName)
        : base(new(DeviceTopologyName.Gamepad, family, deviceName))
    {
    }

    #endregion
}
