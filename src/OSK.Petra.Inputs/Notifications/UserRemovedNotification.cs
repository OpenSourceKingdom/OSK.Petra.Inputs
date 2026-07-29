using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

public class UserRemovedNotification(IInputUser user): UserNotification(user)
{
}
