using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UserActiveSchemeChangeNotification(IInputUser user, ActiveSchemeDetails scheme): UserNotification(user)
{
    public ActiveSchemeDetails NewScheme => scheme;
}
