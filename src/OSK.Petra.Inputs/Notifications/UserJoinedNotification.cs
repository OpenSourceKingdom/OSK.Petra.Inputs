using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

public class UserJoinedNotification(IInputUser user): UserNotification(user)
{
}
