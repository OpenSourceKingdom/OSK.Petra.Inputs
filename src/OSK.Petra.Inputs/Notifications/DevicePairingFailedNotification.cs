using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when device pairing fails.
/// </summary>
public class DevicePairingFailedNotification(int pairingUserId, RuntimeDeviceIdentifier deviceIdentifier)
    : DeviceNotification(deviceIdentifier)
{
    /// <summary>
    /// The ID of the user the pairing attempt failed for.
    /// </summary>
    public int AttemptedPairingUserId => pairingUserId;
}
