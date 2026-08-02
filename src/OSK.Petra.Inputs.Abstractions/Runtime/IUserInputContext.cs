using OSK.Petra.Inputs.Abstractions.Inputs;
using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IUserInputContext
{
    #region Variables

    int UserId { get; }

    RuntimeDeviceIdentifier DeviceIdentifier { get; }

    TState GetOrCreateState<TState>(IInput input, Func<IInput, TState> factory)
        where TState : InputState;

    void SetFeature<TData>(TData data)
        where TData : CapabilityData;

    #endregion
}
