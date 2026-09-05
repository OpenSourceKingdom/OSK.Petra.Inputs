using OSK.Extensions.Petra.Inputs.Devices.Models;

namespace OSK.Extensions.Petra.Inputs.Devices.Gamepads;

/// <summary>
/// Defines a standard button for a gamepad
/// </summary>
/// <param name="id">The id for the input</param>
public abstract class GamepadButton(long id)
    : DigitalInput(id, allowReactivation: true), IGamepadInput
{
}
