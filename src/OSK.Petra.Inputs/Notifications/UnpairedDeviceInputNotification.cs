using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when an unpaired device input is received
/// </summary>
public class UnpairedDeviceInputNotification(RuntimeDeviceIdentifier device, long inputId): DeviceNotification(device)
{
    /// <summary>
    /// The input id for the unpaired input.
    /// </summary>
    public long InputId => inputId;
}
