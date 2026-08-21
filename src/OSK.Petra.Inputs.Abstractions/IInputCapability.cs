using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Abstractions;

public interface IInputCapability
{
    bool CanProcess(IInputEvent inputEvent);

    void Process(IDeviceInputContext context, IInputState state, IInputEvent inputEvent, TimeSpan deltaTime);
}
