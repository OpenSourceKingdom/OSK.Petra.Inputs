using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// An options provider for <see cref="CapabilityOptions"/>. This allows DI while using the <see cref="InputSystemConfiguration"/> as the source of truth for an 
/// input system
/// </summary>
public interface ICapabilityOptionsProvider
{
    /// <summary>
    /// Gets the options for a capability to use
    /// </summary>
    /// <typeparam name="TCapabilityOptions">The type of options the capability uses</typeparam>
    /// <returns>The configured capability options</returns>
    TCapabilityOptions Get<TCapabilityOptions>()
        where TCapabilityOptions: CapabilityOptions, new();
}
