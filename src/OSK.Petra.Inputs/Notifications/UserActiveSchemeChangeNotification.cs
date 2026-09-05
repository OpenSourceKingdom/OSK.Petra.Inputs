using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a user's active input scheme changes.
/// </summary>
public class UserActiveSchemeChangeNotification(IInputUser user, InputConfiguration configuration, string definitionName, string schemeName): UserNotification(user)
{
    /// <summary>
    /// The input configuration the active scheme belongs to.
    /// </summary>
    public InputConfiguration Configuration => configuration;

    /// <summary>
    /// The name of the action definition containing the active scheme.
    /// </summary>
    public string DefinitionName => definitionName;

    /// <summary>
    /// The name of the new active input scheme.
    /// </summary>
    public string SchemeName => schemeName;
}
