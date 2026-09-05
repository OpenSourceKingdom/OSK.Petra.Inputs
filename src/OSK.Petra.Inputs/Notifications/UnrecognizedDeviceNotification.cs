using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted for input from a device that is not recognized or configured with the input system.
/// </summary>
public class UnrecognizedDeviceNotification(RuntimeDeviceIdentifier device): DeviceNotification(device)
{
}
