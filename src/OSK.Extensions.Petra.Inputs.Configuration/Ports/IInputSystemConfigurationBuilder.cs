using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

/// <summary>
/// Builder for creating and configuring an input system configuration.
/// </summary>
public interface IInputSystemConfigurationBuilder
{
    /// <summary>
    /// Sets the input system join policy for user and device pairing behavior.
    /// </summary>
    /// <param name="policy">The join policy to apply</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurationBuilder WithJoinPolicy(InputSystemJoinPolicy policy);

    /// <summary>
    /// Adds an action definition to the configuration.
    /// </summary>
    /// <param name="definition">The action definition to add</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurationBuilder WithActionDefinition(ActionDefinition definition);

    /// <summary>
    /// Adds an input scheme to the configuration.
    /// </summary>
    /// <param name="scheme">The input scheme to add</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurationBuilder WithInputScheme(InputScheme scheme);

    /// <summary>
    /// Configures capability-specific options.
    /// </summary>
    /// <typeparam name="TOptions">The capability options type to configure</typeparam>
    /// <param name="optionsConfigurator">Configuration callback for the options</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurationBuilder WithCapabilityOptions<TOptions>(Action<TOptions> optionsConfigurator)
        where TOptions : CapabilityOptions, new();
}
