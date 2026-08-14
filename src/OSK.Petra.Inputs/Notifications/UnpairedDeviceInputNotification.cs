using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Notifications;

public class UnpairedDeviceInputNotification(RuntimeDeviceIdentifier device, IInput input): DeviceNotification(device)
{
    public IInput Input => input;
}
