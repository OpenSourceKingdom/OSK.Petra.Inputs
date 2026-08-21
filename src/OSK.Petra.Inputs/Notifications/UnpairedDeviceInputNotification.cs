using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UnpairedDeviceInputNotification(RuntimeDeviceIdentifier device, int inputId): DeviceNotification(device)
{
    public int InputId => inputId;
}
