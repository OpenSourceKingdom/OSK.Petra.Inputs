using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a user is removed from the input system.
/// </summary>
public class UserRemovedNotification(IInputUser user): UserNotification(user)
{
}
