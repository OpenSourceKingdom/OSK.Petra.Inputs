using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Notifications;

public class UnrecognizedDeviceInputNotification(RuntimeDeviceIdentifier device, IInput input) : UnrecognizedDeviceNotification(device)
{
    public IInput Input => input;
}
