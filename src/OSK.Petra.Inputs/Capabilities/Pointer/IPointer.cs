using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// Interface for a device input representing pointer/mouse-like motion and interaction.
/// </summary>
public interface IPointer: IDeviceInput, IInput<PointerSettings>
{
}
