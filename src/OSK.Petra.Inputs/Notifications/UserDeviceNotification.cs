using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public abstract class UserDeviceNotification(IInputUser user, RuntimeDeviceIdentifier deviceIdentifier)
    : DeviceNotification(deviceIdentifier)
{
    public IInputUser User => user;
}
