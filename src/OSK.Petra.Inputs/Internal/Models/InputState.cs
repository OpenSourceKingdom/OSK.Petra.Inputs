using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Internal.Models;

internal class InputState(IInput input, DeviceInputContext deviceContext) : IInputState
{
    #region Variables

    internal bool IsDisposed { get; private set; }

    private readonly Dictionary<Type, ICapabilityDetail> _detailLookup = [];

    #endregion

    #region IInputState

    public IInput Input => input;

    public InputPhase Phase { get; set; }

    public TimeSpan Duration { get; internal set; }

    public bool Consumed { get; set; }

    public TDetail? GetDetail<TDetail>() 
        where TDetail : ICapabilityDetail
        => _detailLookup.TryGetValue(typeof(TDetail), out var detail) && detail is TDetail typedDetail
            ? typedDetail
            : default;

    public void SetDetail<TDetail>(TDetail detail) 
        where TDetail : ICapabilityDetail
    {
        if (detail is null)
        {
            throw new ArgumentNullException(nameof(detail));
        }

        _detailLookup[typeof(TDetail)] = detail;
    }

    public void Dispose()
    {
        deviceContext.RemoveState(this);
        IsDisposed = true;
    }

    #endregion
}
