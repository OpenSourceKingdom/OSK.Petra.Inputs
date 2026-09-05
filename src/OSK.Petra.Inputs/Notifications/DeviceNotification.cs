using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Base class for notifications related to a specific device.
/// </summary>
public abstract class DeviceNotification(RuntimeDeviceIdentifier deviceIdentifier): IInputSystemNotification
{
    /// <summary>
    /// The device identifier associated with this notification.
    /// </summary>
    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;
}
