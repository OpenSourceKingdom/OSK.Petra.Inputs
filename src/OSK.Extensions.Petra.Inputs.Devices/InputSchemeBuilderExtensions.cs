using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Extensions.Petra.Inputs.Devices.Gamepads;
using OSK.Extensions.Petra.Inputs.Devices.Keyboards;
using OSK.Extensions.Petra.Inputs.Devices.Mice;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices;

public static class InputSchemeBuilderExtensions
{
    extension(IInputSchemeBuilder builder)
    {
        public IInputSchemeBuilder WithKeyboard(IKeyboardInput keyboardInput, string actionName, DeviceFamily? family = null, string deviceName = DeviceIdentities.GenericDeviceName)
            => builder.WithMap(
                DeviceIdentities.Keyboard(family ?? DeviceFamily.Generic, string.IsNullOrWhiteSpace(deviceName) ? DeviceIdentities.GenericDeviceName : deviceName),
                keyboardInput.Id,
                actionName
            );

        public IInputSchemeBuilder WithGamepad(IGamepadInput gamepadInput, string actionName, DeviceFamily? family = null, string deviceName = DeviceIdentities.GenericDeviceName)
            => builder.WithMap(
                DeviceIdentities.Gamepad(family ?? DeviceFamily.Generic, string.IsNullOrWhiteSpace(deviceName) ? DeviceIdentities.GenericDeviceName : deviceName),
                gamepadInput.Id,
                actionName
            );

        public IInputSchemeBuilder WithMouse<TTopology>(IMouseInput mouseInput, string actionName, DeviceFamily? family = null, string deviceName = DeviceIdentities.GenericDeviceName)
            => builder.WithMap(
                DeviceIdentities.Mouse(family ?? DeviceFamily.Generic, string.IsNullOrWhiteSpace(deviceName) ? DeviceIdentities.GenericDeviceName : deviceName),
                mouseInput.Id,
                actionName
            );
    }
}
