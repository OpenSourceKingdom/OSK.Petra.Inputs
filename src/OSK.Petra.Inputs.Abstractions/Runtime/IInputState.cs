using System;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IInputState: IDisposable
{
    event Action<IInputState>? Disposed;

    bool IsDisposed { get; }

    bool IsNewActivation { get; }

    IInput Input { get; }

    InputPhase Phase { get; }

    TimeSpan Duration { get; }

    public IInput? InputConsumer { get; set; }

    void CombinePhase(InputPhase phase);

    bool TryConsume(IInputState state); 

    void SetDetails<TDetail>(TDetail detail)
        where TDetail: ICapabilityDetails;

    TDetail? GetDetails<TDetail>()
        where TDetail: ICapabilityDetails;
}
