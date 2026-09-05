namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when the input monitoring status changes.
/// </summary>
public class InputMonitorStatusChangedNotification(bool isMonitoringInput): SystemNotification
{
    /// <summary>
    /// Whether the input system is currently monitoring for input.
    /// </summary>
    public bool IsMonitoringInput => isMonitoringInput;
}
