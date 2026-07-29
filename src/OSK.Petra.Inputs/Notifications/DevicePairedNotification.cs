using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class DevicePairedNotification(int userId, RuntimeDeviceIdentifier deviceIdentifier)
    : DeviceNotification(deviceIdentifier)
{
    public int PairedUserId => userId;
}
