using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a new user joins the input system.
/// </summary>
public class UserJoinedNotification(IInputUser user): UserNotification(user)
{
}
