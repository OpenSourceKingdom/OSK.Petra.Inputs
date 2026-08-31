using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UnrecognizedDeviceInputNotification(RuntimeDeviceIdentifier device, long? inputId) : UnrecognizedDeviceNotification(device)
{
    public long? InputId => inputId;
}
