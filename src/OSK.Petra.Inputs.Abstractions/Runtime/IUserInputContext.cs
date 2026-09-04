using OSK.Petra.Inputs.Abstractions.Devices;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// Provides access to a user's input state and virtual input context during event processing.
/// </summary>
public interface IUserInputContext
{
    #region Variables

    /// <summary>
    /// Gets the user ID for this context.
    /// </summary>
    int UserId { get; }

    /// <summary>
    /// Gets the virtual input context for managing virtual input states.
    /// </summary>
    IVirtualInputContext VirtualInputContext { get; }

    /// <summary>
    /// Attempts to get an input state for a device input.
    /// </summary>
    /// <param name="identity">The device to get the state for</param>
    /// <param name="inputId">The specific input on the device</param>
    /// <param name="state">The state, if it exists</param>
    /// <returns>Whether the state existed</returns>
    bool TryGetInputState(DeviceIdentity identity, long inputId, [NotNullWhen(true)] out IInputState? state);

    /// <summary>
    /// Stores a capability feature in the context.
    /// </summary>
    /// <typeparam name="TFeature">The type of feature being set in the context</typeparam>
    /// <param name="feature">The feature data to set</param>
    void SetFeature<TFeature>(TFeature feature)
        where TFeature : ICapabilityFeature;

    /// <summary>
    /// Retrieves a capability feature from the context.
    /// </summary>
    /// <typeparam name="TFeature">The type of feature to get</typeparam>
    /// <returns>The feature, if it exists</returns>
    TFeature? GetFeature<TFeature>()
        where TFeature : ICapabilityFeature;

    #endregion
}
