using OSK.Extensions.Petra.Inputs.Devices.Models;

namespace OSK.Extensions.Petra.Inputs.Devices.Keyboards;

public abstract class KeyboardKey(long id): DigitalInput(id, allowReactivation: true), IKeyboardInput
{
}
