using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Base class for user notifications
/// </summary>
public abstract class UserNotification(IInputUser user): IInputSystemNotification
{
    /// <summary>
    /// The user associated with this notification.
    /// </summary>
    public IInputUser User => user;
}
