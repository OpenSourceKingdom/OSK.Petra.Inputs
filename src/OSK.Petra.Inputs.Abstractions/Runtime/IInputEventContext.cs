using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// The contextual information that is generated from an activated input for an action to utilize when being executed
/// </summary>
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

    /// <summary>
    /// The source that triggered the input
    /// </summary>
    public InputOriginationSource OriginationSource { get; }

    /// <summary>
    /// Attempts to get a specifc set of input details for a given input. Whether the details are available will depend on the input events that are triggered with the input
    /// </summary>
    /// <typeparam name="TDetails">The type of capability details to get</typeparam>
    /// <returns>The detail information, if it was processed with the input, otherwise null</returns>
    TDetails? GetInputDetails<TDetails>()
        where TDetails: ICapabilityDetails;

    /// <summary>
    /// Attempts to get a specific input capability feature that a user possesses. Whether the feature is avialable will depend on the inputs the user has on their devices
    /// </summary>
    /// <typeparam name="TFeature">The type of feature to get</typeparam>
    /// <returns>The feature information, if it was processed for the user</returns>
    TFeature? GetInputFeature<TFeature>()
        where TFeature: ICapabilityFeature;

    /// <summary>
    /// The services associated with this event context
    /// </summary>
    IServiceProvider Services { get; }
}
