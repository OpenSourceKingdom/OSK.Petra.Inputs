using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Notifications;

public class DeviceInputNotification(RuntimeDeviceIdentifier deviceIdentifier, long inputId, TimeSpan deltaTime, params IInputEvent[] inputEvents): DeviceNotification(deviceIdentifier)
{
    public long InputId => inputId;

    public TimeSpan DeltaTime => deltaTime;

    public IInputEvent[] InputEvents => inputEvents;
}
