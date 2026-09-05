using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a user's active action definition changes.
/// </summary>
public class UserActiveDefinitionChangeNotification(IInputUser user, string definitionName): UserNotification(user)
{
    /// <summary>
    /// The name of the new active action definition.
    /// </summary>
    public string ActiveDefinitionName => definitionName;
}
