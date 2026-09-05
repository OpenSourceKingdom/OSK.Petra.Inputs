using OSK.Extensions.Petra.Inputs.Devices.Models;

namespace OSK.Extensions.Petra.Inputs.Devices.Mice;

/// <summary>
/// Defines an input for mouse motion (e.g. pointer)
/// </summary>
/// <param name="id">The input id</param>
/// <param name="distanceThreshold">The distance the motion must travel to be considered a full, intentional movement</param>
public abstract class MouseMotionInput(long id, float distanceThreshold = 0.1f): PointerInput(id, distanceThreshold), IMouseInput
{
}
