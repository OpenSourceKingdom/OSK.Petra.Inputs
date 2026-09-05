using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Base class for notifications related to devices a user possesses
/// </summary>
public abstract class UserDeviceNotification(IInputUser user, RuntimeDeviceIdentifier deviceIdentifier)
    : DeviceNotification(deviceIdentifier)
{
    /// <summary>
    /// The user associated with this notification.
    /// </summary>
    public IInputUser User => user;
}
