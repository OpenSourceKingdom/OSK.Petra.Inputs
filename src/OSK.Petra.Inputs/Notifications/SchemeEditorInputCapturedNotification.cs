using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Notifications;

internal class SchemeEditorInputCapturedNotification(int userId, DeviceIdentity deviceIdentity, IInput input): SchemeEditorNotification
{
    public int UserId => userId;

    public DeviceIdentity DeviceIdentity => deviceIdentity;

    public IInput Input => input;
}
