namespace OSK.Petra.Inputs.Notifications;

public class ActionExecutedNotification(int userId, string definitionName, string actionName): SystemNotification
{
    public int UserId { get; } = userId;

    public string DefinitionName { get; } = definitionName;

    public string ActionName { get; } = actionName;
}
