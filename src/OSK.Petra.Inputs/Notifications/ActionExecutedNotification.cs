namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when a user action is executed by the input system.
/// </summary>
public class ActionExecutedNotification(int userId, string definitionName, string actionName): SystemNotification
{
    /// <summary>
    /// The ID of the user who executed the action.
    /// </summary>
    public int UserId { get; } = userId;

    /// <summary>
    /// The name of the action definition containing the executed action.
    /// </summary>
    public string DefinitionName { get; } = definitionName;

    /// <summary>
    /// The name of the action that was executed.
    /// </summary>
    public string ActionName { get; } = actionName;
}
