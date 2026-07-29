namespace OSK.Petra.Inputs.Notifications;

public class InputMonitorStatusChangedNotification(bool isMonitoringInput): SystemNotification
{
    public bool IsMonitoringInput => isMonitoringInput;
}
