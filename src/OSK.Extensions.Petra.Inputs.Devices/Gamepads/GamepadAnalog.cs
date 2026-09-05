using OSK.Extensions.Petra.Inputs.Devices.Models;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Extensions.Petra.Inputs.Devices.Gamepads;

/// <summary>
/// Defines a standard power/analog input for a gamepad
/// </summary>
/// <param name="id">The id for the input</param>
/// <param name="powerAxis">The axis the input aligns with</param>
/// <param name="sensitivityThreshold">The amount of power required to register as a full, intentional activation</param>
public abstract class GamepadAnalog(long id, PowerAxis powerAxis, float sensitivityThreshold = 0.1f) 
    : AnalogInput(id, powerAxis, sensitivityThreshold), IGamepadInput
{
}
