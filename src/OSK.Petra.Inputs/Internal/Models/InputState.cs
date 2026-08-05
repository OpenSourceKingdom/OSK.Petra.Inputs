using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Internal.Models;

internal class InputState(IInput input, DeviceInputContext deviceContext) : IInputState
{
    #region Variables

    internal bool IsDisposed { get; private set; }

    private readonly Dictionary<Type, ICapabilityDetails> _detailLookup = [];

    #endregion

    #region IInputState

    public event Action<IInputState>? Disposed;

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceContext.DeviceIdentifier;

    public IInput Input => input;

    public bool IsNewActivation { get; internal set; }

    public InputPhase Phase { get; private set; }

    public TimeSpan Duration { get; internal set; }

    public TDetails? GetDetails<TDetails>() 
        where TDetails : ICapabilityDetails
        => _detailLookup.TryGetValue(typeof(TDetails), out var detail) && detail is TDetails typedDetail
            ? typedDetail
            : default;

    public void SetDetails<TDetails>(TDetails detail) 
        where TDetails : ICapabilityDetails
    {
        if (detail is null)
        {
            throw new ArgumentNullException(nameof(detail));
        }

        _detailLookup[typeof(TDetails)] = detail;
    }

    public void CombinePhase(InputPhase phase)
    {
        Phase = Phase.Combine(phase);
    }

    public void Dispose()
    {
        deviceContext.RemoveState(this);
        IsDisposed = true;

        Disposed?.Invoke(this);
    }

    #endregion
}
