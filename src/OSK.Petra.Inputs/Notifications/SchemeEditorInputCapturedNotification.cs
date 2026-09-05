using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when the scheme editor successfully captures input.
/// </summary>
internal class SchemeEditorInputCapturedNotification(int userId, DeviceIdentity deviceIdentity, IDeviceInput input): SchemeEditorNotification
{
    /// <summary>
    /// The ID of the user that captured the input.
    /// </summary>
    public int UserId => userId;

    /// <summary>
    /// The device identity where the input was captured.
    /// </summary>
    public DeviceIdentity DeviceIdentity => deviceIdentity;

    /// <summary>
    /// The captured device input.
    /// </summary>
    public IDeviceInput Input => input;
}
