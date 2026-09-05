using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a device connects to a user.
/// </summary>
public class UserDeviceConnectedNotification(IInputUser user, RuntimeDeviceIdentifier deviceIdentifier)
    : UserDeviceNotification(user, deviceIdentifier)
{
}
