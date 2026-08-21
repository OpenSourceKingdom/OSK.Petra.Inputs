using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Notifications;

public class DeviceInputNotification(RuntimeDeviceIdentifier deviceIdentifier, int inputId, TimeSpan deltaTime, params IInputEvent[] inputEvents): DeviceNotification(deviceIdentifier)
{
    public int InputId => inputId;

    public TimeSpan DeltaTime => deltaTime;

    public IInputEvent[] InputEvents => inputEvents;
}
