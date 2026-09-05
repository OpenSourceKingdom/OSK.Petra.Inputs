using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Capabilities.Power;

/// <summary>
/// Represents an input with power based capability
/// (e.g., triggers, buttons).
/// </summary>
public interface IPowerInput: IDeviceInput, IInput<PowerSettings>
{
}
