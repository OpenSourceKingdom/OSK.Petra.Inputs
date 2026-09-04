using OSK.Petra.Inputs.Abstractions.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IVirtualInputContext
{
    IEnumerable<TInput> GetInputs<TInput>()
        where TInput : IVirtualInput;

    bool TryGetState(IVirtualInput virtualInput, [NotNullWhen(true)] out IInputState? state);

    IInputState GetOrCreateState(IVirtualInput virtualInput, Func<IInputEvent[]> inputEventsFactory);
}
