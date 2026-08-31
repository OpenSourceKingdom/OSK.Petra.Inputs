namespace OSK.Petra.Inputs.Notifications;

public class InputSystemFocusNotification(bool hasFocus): SystemNotification
{
    public bool HasFocus => hasFocus;
}
