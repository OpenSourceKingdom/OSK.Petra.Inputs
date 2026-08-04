namespace OSK.Petra.Inputs.Notifications;

public class SchemeEditorInputCaptureTimeoutNotification(int userId): SchemeEditorNotification
{
    public int UserId => userId;
}
