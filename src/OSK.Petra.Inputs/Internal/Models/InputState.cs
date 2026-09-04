using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Internal.Models;

internal abstract class InputState(IInput input) : IInputState
{
    #region Variables

    internal IInputEvent[] LastReceivedEvents { get; set; } = [];

    private readonly Dictionary<Type, ICapabilityDetails> _detailLookup = [];

    private bool _hasStatusBeenSet;

    #endregion

    #region IInputState

    public event Action<IInputState>? Disposed;

    public bool IsDisposed { get; private set; }

    public IInput? InputConsumer { get; set; }

    public IInput Input => input;

    public bool IsNewActivation { get; private set; } = true;

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

    public bool TryConsume(IInputState state)
    {
        if (InputConsumer is not null)
        {
            return false;
        }
        if (state.InputConsumer is not null)
        {
            return state.InputConsumer == Input;
        }

        state.InputConsumer = Input;

        state.Disposed += _ =>
        {
            if (!IsDisposed)
            {
                Dispose();
            }
        };
        Disposed += _ =>
        {
            if (!state.IsDisposed)
            {
                state.Dispose();
            }
        };

        return true;
    }

    public void CombinePhase(InputPhase phase)
    {
        Phase = _hasStatusBeenSet
            ? Phase.Combine(phase)
            : phase;

        _hasStatusBeenSet = true;
    }

    public void Reset()
    {
        IsNewActivation = false;
        _hasStatusBeenSet = false;
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        OnDispose();
        IsDisposed = true;
        Disposed?.Invoke(this);
    }

    #endregion

    #region Helpers

    public abstract InputOriginationSource GetOriginationSource();

    protected abstract void OnDispose();

    #endregion
}
