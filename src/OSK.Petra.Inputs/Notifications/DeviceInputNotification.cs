using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Notifications;

public class DeviceInputNotification(RuntimeDeviceIdentifier deviceIdentifier, IInput input): DeviceNotification(deviceIdentifier)
{
    public IInput Input => input;
}
