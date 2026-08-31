using System;
using OSK.Petra.Inputs.Abstractions.Devices;

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

    TDetails? GetInputDetails<TDetails>()
        where TDetails: ICapabilityDetails;

    TFeature? GetDeviceFeature<TFeature>()
        where TFeature: ICapabilityFeature;

    /// <summary>
    /// The services associated with this event context
    /// </summary>
    IServiceProvider Services { get; }
}
