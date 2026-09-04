using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Notifications;

internal class SchemeEditorInputCapturedNotification(int userId, DeviceIdentity deviceIdentity, IDeviceInput input): SchemeEditorNotification
{
    public int UserId => userId;

    public DeviceIdentity DeviceIdentity => deviceIdentity;

    public IDeviceInput Input => input;
}
