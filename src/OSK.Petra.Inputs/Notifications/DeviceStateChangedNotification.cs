using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class DeviceStateChangedNotification(RuntimeDeviceIdentifier deviceIdentifier, DeviceStatus status)
    : UnrecognizedDeviceNotification(deviceIdentifier)
{
    public DeviceStatus Status => status;
}
