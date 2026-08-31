using System;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IInputState: IDisposable
{
    event Action<IInputState>? Disposed;

    bool IsNewActivation { get; }

    RuntimeDeviceIdentifier DeviceIdentifier { get; }

    IInput Input { get; }

    InputPhase Phase { get; }

    TimeSpan Duration { get; }

    void CombinePhase(InputPhase phase);

    void SetDetails<TDetail>(TDetail detail)
        where TDetail: ICapabilityDetails;

    TDetail? GetDetails<TDetail>()
        where TDetail: ICapabilityDetails;
}
