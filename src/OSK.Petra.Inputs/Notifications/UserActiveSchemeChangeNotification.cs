using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UserActiveSchemeChangeNotification(IInputUser user, ActiveInputScheme scheme): UserNotification(user)
{
    public ActiveInputScheme NewScheme => scheme;
}
