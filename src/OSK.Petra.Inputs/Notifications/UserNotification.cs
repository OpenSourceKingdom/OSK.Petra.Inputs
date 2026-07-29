using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

public abstract class UserNotification(IInputUser user): IInputSystemNotification
{
    public IInputUser User => user;
}
