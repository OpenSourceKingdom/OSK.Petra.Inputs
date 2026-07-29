using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public abstract class DeviceNotification(RuntimeDeviceIdentifier deviceIdentifier): IInputSystemNotification
{
    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;
}
