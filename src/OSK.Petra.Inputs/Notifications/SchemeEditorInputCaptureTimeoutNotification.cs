namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when input capture times out waiting for user input.
/// </summary>
public class SchemeEditorInputCaptureTimeoutNotification(int userId): SchemeEditorNotification
{
    /// <summary>
    /// The ID of the user whose input capture timed out.
    /// </summary>
    public int UserId => userId;
}
