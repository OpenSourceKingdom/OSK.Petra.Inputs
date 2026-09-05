using OSK.Extensions.Petra.Inputs.Devices.Models;

namespace OSK.Extensions.Petra.Inputs.Devices.Mice;

/// <summary>
/// Defines a standard mouse button
/// </summary>
/// <param name="id">The id for the input</param>
public abstract class MouseButton(long id): DigitalInput(id, allowReactivation: true), IMouseInput
{
}
