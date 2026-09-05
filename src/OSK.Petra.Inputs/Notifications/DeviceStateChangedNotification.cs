using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when the state of a device changes.
/// </summary>
public class DeviceStateChangedNotification(RuntimeDeviceIdentifier deviceIdentifier, DeviceStatus status)
    : UnrecognizedDeviceNotification(deviceIdentifier)
{
    /// <summary>
    /// The new status of the device.
    /// </summary>
    public DeviceStatus Status => status;
}
