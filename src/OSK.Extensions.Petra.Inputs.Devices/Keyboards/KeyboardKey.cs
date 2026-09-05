using OSK.Extensions.Petra.Inputs.Devices.Models;

namespace OSK.Extensions.Petra.Inputs.Devices.Keyboards;

/// <summary>
/// Defines a standard keyboard input
/// </summary>
/// <param name="id">The id for the input</param>
public abstract class KeyboardKey(long id): DigitalInput(id, allowReactivation: true), IKeyboardInput
{
}
