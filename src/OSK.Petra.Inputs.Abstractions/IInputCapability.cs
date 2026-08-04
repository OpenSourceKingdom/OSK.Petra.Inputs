using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Abstractions;

public interface IInputCapability
{
    bool CanProces(IInput input);

    void Process(IDeviceInputContext context, IInputState state, TimeSpan deltaTime);
}
