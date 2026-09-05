using OSK.Hexagonal.MetaData;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Abstractions;

/// <summary>
/// Processes input events and manages state updates for a specific input capability.
/// </summary>
/// <remarks>
/// 💡Notes:
/// <list type="bullet">
/// <item>Capabilities are provided by the library, but custom capabilities can also be added to the DI chain and utilized independently of any official capability.</item>
/// </list>
/// </remarks>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired, HexagonalIntegrationType.LibraryProvided)]
public interface IInputCapability
{
    /// <summary>
    /// Determines whether this capability can process the provided input event.
    /// </summary>
    /// <param name="inputEvent">The input event to validate</param>
    /// <returns>Whether the capability can process the event or not</returns>
    bool CanProcess(IInputEvent inputEvent);

    /// <summary>
    /// Processes an input event by updating state and executing capability specific logic.
    /// </summary>
    /// <param name="context">The input context for the user</param>
    /// <param name="state">The state of the input that triggered the event</param>
    /// <param name="inputEvent">The event to process for the input</param>
    /// <param name="deltaTime">The amount of time that passed since the previous frame</param>
    void Process(IUserInputContext context, IInputState state, IInputEvent inputEvent, TimeSpan deltaTime);
}
