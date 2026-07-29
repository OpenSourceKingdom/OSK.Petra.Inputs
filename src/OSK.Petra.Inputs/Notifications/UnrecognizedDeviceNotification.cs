using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UnrecognizedDeviceNotification(RuntimeDeviceIdentifier device): DeviceNotification(device)
{
}
