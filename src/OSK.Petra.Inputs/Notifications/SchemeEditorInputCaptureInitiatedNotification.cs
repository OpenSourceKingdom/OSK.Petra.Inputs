using System;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when input capture is initiated by the scheme editor.
/// </summary>
public class SchemeEditorInputCaptureInitiatedNotification(int userId, TimeSpan? captureTimeout): SchemeEditorNotification
{
    /// <summary>
    /// The ID of the user initiating input capture.
    /// </summary>
    public int UserId => userId;

    /// <summary>
    /// The timeout duration for input capture, if specified.
    /// </summary>
    public TimeSpan? CaptureTimeout => captureTimeout;
}
