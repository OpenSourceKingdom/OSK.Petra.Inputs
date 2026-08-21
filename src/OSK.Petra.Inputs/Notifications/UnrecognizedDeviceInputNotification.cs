using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UnrecognizedDeviceInputNotification(RuntimeDeviceIdentifier device, int? inputId) : UnrecognizedDeviceNotification(device)
{
    public int? InputId => inputId;
}
