using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Extensions.Petra.Inputs.Devices.Gamepads;
using OSK.Extensions.Petra.Inputs.Devices.Keyboards;
using OSK.Extensions.Petra.Inputs.Devices.Mice;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Devices;

public static class InputSchemeBuilderExtensions
{
    extension(IInputSchemeBuilder builder)
    {
        public IInputSchemeBuilder WithKeyboard(IKeyboardInput keyboardInput, string actionName, DeviceFamily? family = null, string deviceName = DeviceNames.Generic)
            => builder.WithMap(
                new DeviceIdentity(DeviceTopologyName.Keyboard, family ?? DeviceFamily.Generic, string.IsNullOrWhiteSpace(deviceName) ? DeviceNames.Generic : deviceName),
                keyboardInput,
                actionName
            );

        public IInputSchemeBuilder WithGamepad(IGamepadInput gamepadInput, string actionName, DeviceFamily? family = null, string deviceName = DeviceNames.Generic)
            => builder.WithMap(
                new DeviceIdentity(DeviceTopologyName.Gamepad, family ?? DeviceFamily.Generic, string.IsNullOrWhiteSpace(deviceName) ? DeviceNames.Generic : deviceName),
                gamepadInput,
                actionName
            );

        public IInputSchemeBuilder WithMouse<TTopology>(IMouseInput mouseInput, string actionName, DeviceFamily? family = null, string deviceName = DeviceNames.Generic)
            => builder.WithMap(
                new DeviceIdentity(DeviceTopologyName.Mouse, family ?? DeviceFamily.Generic, string.IsNullOrWhiteSpace(deviceName) ? DeviceNames.Generic : deviceName),
                mouseInput,
                actionName
            );
    }
}
