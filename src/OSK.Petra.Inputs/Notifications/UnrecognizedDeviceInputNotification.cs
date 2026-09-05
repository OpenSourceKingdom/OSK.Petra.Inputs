using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notification transmitted when input is received from an unrecognized device.
/// </summary>
public class UnrecognizedDeviceInputNotification(RuntimeDeviceIdentifier device, long? inputId) : UnrecognizedDeviceNotification(device)
{
    /// <summary>
    /// The input identifier, if available, or null if not captured.
    /// </summary>
    public long? InputId => inputId;
}
