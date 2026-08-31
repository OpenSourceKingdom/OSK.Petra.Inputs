using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IInputSystemConfigurationBuilder
{
    /// <summary>
    /// Sets the input system join policy
    /// </summary>
    /// <param name="policy">The policy</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurationBuilder WithJoinPolicy(InputSystemJoinPolicy policy);

    IInputSystemConfigurationBuilder WithActionDefinition(ActionDefinition definition);

    IInputSystemConfigurationBuilder WithInputScheme(InputScheme scheme);

    IInputSystemConfigurationBuilder WithCapabilityOptions<TOptions>(Action<TOptions> optionsConfigurator)
        where TOptions : CapabilityOptions, new();
}
