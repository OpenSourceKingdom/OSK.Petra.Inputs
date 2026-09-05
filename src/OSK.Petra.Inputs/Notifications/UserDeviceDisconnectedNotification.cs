using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a device disconnects from a user.
/// </summary>
public class UserDeviceDisconnectedNotification(IInputUser user, RuntimeDeviceIdentifier deviceIdentifier)
    : UserDeviceNotification(user, deviceIdentifier)
{
}
