using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class UserDeviceConnectedNotification(IInputUser user, RuntimeDeviceIdentifier deviceIdentifier)
    : UserDeviceNotification(user, deviceIdentifier)
{
}
