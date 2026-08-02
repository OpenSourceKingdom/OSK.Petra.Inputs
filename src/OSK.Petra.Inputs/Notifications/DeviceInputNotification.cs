using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Notifications;

public class DeviceInputNotification(RuntimeDeviceIdentifier deviceIdentifier, IInput input, TimeSpan deltaTime, int[]? suppressedActionGroups = null): DeviceNotification(deviceIdentifier)
{
    public IInput Input => input;

    public TimeSpan DeltaTime => deltaTime;

    public int[] SuppressedActionGroups { get; } = suppressedActionGroups ?? [];
}
