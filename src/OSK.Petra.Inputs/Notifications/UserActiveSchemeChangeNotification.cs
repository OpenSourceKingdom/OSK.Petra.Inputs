using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Notifications;

public class UserActiveSchemeChangeNotification(IInputUser user, InputConfiguration configuration, string definitionName, string schemeName): UserNotification(user)
{
    public InputConfiguration Configuration => configuration;

    public string DefinitionName => definitionName;

    public string SchemeName => schemeName;
}
