using OSK.Petra.Inputs.Abstractions.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// Manages virtual input states and retrieval during event processing.
/// </summary>
public interface IVirtualInputContext
{
    /// <summary>
    /// Gets all virtual inputs of a specific type.
    /// </summary>
    IEnumerable<TInput> GetInputs<TInput>()
        where TInput : IVirtualInput;

    /// <summary>
    /// Attempts to get the current state of a virtual input.
    /// </summary>
    /// <param name="virtualInput">The virtual input to get the state for</param>
    /// <param name="state">The state, if it exists</param>
    /// <returns>Whether the state existed</returns>
    bool TryGetState(IVirtualInput virtualInput, [NotNullWhen(true)] out IInputState? state);

    /// <summary>
    /// Gets or creates a new input state for a virtual input.
    /// </summary>
    /// <param name="virtualInput">The virtual input used for the state</param>
    /// <param name="inputEventsFactory">The factory that is used if the virtual input does not currently have state information</param>
    /// <returns>The input state</returns>
    IInputState GetOrCreateState(IVirtualInput virtualInput, Func<IInputEvent[]> inputEventsFactory);
}
