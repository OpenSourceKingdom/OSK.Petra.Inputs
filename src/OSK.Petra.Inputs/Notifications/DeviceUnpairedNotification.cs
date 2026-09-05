using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a device is unpaired from a user.
/// </summary>
public class DeviceUnpairedNotification(int userId, RuntimeDeviceIdentifier deviceIdentifier)
    : DeviceNotification(deviceIdentifier)
{
    /// <summary>
    /// The ID of the user the device was unpaired from.
    /// </summary>
    public int UnpairedUserId => userId;
}
