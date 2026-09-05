namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when the input system gains or loses application focus.
/// </summary>
public class InputSystemFocusNotification(bool hasFocus): SystemNotification
{
    /// <summary>
    /// Whether the application has input focus.
    /// </summary>
    public bool HasFocus => hasFocus;
}
