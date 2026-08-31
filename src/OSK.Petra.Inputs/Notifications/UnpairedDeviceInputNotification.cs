using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UnpairedDeviceInputNotification(RuntimeDeviceIdentifier device, long inputId): DeviceNotification(device)
{
    public long InputId => inputId;
}
