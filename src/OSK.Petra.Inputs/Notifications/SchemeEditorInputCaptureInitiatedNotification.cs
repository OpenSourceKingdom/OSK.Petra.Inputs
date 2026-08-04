using System;

namespace OSK.Petra.Inputs.Notifications;

public class SchemeEditorInputCaptureInitiatedNotification(int userId, TimeSpan? captureTimeout): SchemeEditorNotification
{
    public int UserId => userId;

    public TimeSpan? CaptureTimeout => captureTimeout;
}
