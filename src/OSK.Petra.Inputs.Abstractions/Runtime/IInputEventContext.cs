using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IInputEventContext
{
    /// <summary>
    /// The user who initiated the event
    /// </summary>
    public int UserId { get; }

    /// <summary>
    /// The amount of time that has occurred since the last frame was processed
    /// </summary>
    public TimeSpan DeltaTime { get; }

    RuntimeDeviceIdentifier DeviceIdentifier { get; }

    IInput Input { get; }

    TDetail? GetInputDetail<TDetail>()
        where TDetail: ICapabilityDetail;

    TFeature? GetDeviceFeature<TFeature>()
        where TFeature: ICapabilityFeature;

    /// <summary>
    /// The services associated with this event context
    /// </summary>
    IServiceProvider Services { get; }
}
