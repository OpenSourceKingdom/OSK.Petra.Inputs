using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a device is successfully paired to a user.
/// </summary>
public class DevicePairedNotification(int userId, RuntimeDeviceIdentifier deviceIdentifier)
    : DeviceNotification(deviceIdentifier)
{
    /// <summary>
    /// The ID of the user the device was paired to.
    /// </summary>
    public int PairedUserId => userId;
}
