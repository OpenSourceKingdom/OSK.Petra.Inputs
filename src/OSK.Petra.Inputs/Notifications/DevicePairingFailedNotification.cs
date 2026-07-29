using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class DevicePairingFailedNotification(int pairingUserId, RuntimeDeviceIdentifier deviceIdentifier)
    : DeviceNotification(deviceIdentifier)
{
    public int AttemptedPairingUserId => pairingUserId;
}
