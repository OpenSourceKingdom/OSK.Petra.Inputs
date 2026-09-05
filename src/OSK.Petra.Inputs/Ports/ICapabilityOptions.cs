using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// Provides a mechanism to retrieve capability specific options.
/// </summary>
/// <typeparam name="TOptions"> The type of capability options to use</typeparam>
public interface ICapabilityOptions<TOptions>
    where TOptions: CapabilityOptions, new()
{
    /// <summary>
    /// The capability options value.
    /// </summary>
    TOptions Value { get; }
}
