using System;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// Represents the data an input contains within the input system as it is processed over the <see cref="InputPhase"/> lifecycle
/// </summary>
public interface IInputState: IDisposable
{
    /// <summary>
    /// An action triggered when the state is removed from the user input context
    /// </summary>
    event Action<IInputState>? Disposed;

    /// <summary>
    /// Whether the input has been disposed.
    /// </summary>
    /// <remarks> 
    /// 💡Notes: 
    /// <list type="bullet"> 
    /// <item>If an input state is disposed, it will be removed from the input system. Input capabilities should utilize this only to know when the input is considered finished and removed in the next frame.</item> 
    /// </list>
    /// </remarks>
    bool IsDisposed { get; }

    /// <summary>
    /// Whether the state represents a new activation. This will only happen on the first frame the input data was received
    /// </summary>
    bool IsNewActivation { get; }

    /// <summary>
    /// The underlying input the state refers to
    /// </summary>
    IInput Input { get; }

    /// <summary>
    /// The current lifecycle phase the input is
    /// </summary>
    InputPhase Phase { get; }

    /// <summary>
    /// The total amount of time that the input has been engaged
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// A link to the input that 'owns' this state, if it is consumed
    /// </summary>
    public IInput? InputConsumer { get; set; }

    /// <summary>
    /// Combines the provided phase into the current phase of the input state
    /// </summary>
    /// <param name="phase">The phase to combine into the current state</param>
    void CombinePhase(InputPhase phase);

    /// <summary>
    /// Attempts to consume the provided input state
    /// </summary>
    /// <param name="state">The state that will be consumed</param>
    /// <returns>Whether the state was successfully consumed</returns>
    bool TryConsume(IInputState state); 

    /// <summary>
    /// Sets input detail information for the given input
    /// </summary>
    /// <typeparam name="TDetail">The type of capability details to apply to this state</typeparam>
    /// <param name="detail">The detail data that the state will contain</param>
    void SetDetails<TDetail>(TDetail detail)
        where TDetail: ICapabilityDetails;

    /// <summary>
    /// Attempts to get the detail information from the state
    /// </summary>
    /// <typeparam name="TDetail">The type of capability details to get</typeparam>
    /// <returns>The detail data, if it exists on the state</returns>
    TDetail? GetDetails<TDetail>()
        where TDetail: ICapabilityDetails;
}
