using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UserDeviceDisconnectedNotification(IInputUser user, RuntimeDeviceIdentifier deviceIdentifier)
    : UserDeviceNotification(user, deviceIdentifier)
{
}
