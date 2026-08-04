using OSK.Petra.Inputs.Abstractions.Inputs;
using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IInputState: IDisposable
{
    IInput Input { get; }

    InputPhase Phase { get; set; }

    TimeSpan Duration { get; }

    public bool Consumed { get; }

    void SetDetail<TDetail>(TDetail detail)
        where TDetail: ICapabilityDetail;

    TDetail? GetDetail<TDetail>()
        where TDetail: ICapabilityDetail;
}
