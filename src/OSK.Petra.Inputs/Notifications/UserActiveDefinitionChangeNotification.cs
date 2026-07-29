using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

public class UserActiveDefinitionChangeNotification(IInputUser user, string definitionName): UserNotification(user)
{
    public string ActiveDefinitionName => definitionName;
}
