using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using System;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IInputSystemBuilder
{
    /// <summary>
    /// Adds a join policy using an action configurator
    /// </summary>
    /// <param name="policyConfigurator">The policy configurator</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemBuilder WithJoinPolicy(Action<InputSystemJoinPolicy> policyConfigurator);

    IInputSystemBuilder WithActionDefinition(ActionDefinition definition);

    IInputSystemBuilder WithDeviceTopology(IDeviceTopology topology);

    IInputSystemBuilder WithInputScheme(InputScheme scheme);

    IInputSystemBuilder UseSchemeRepository<T>()
        where T : ISchemeRepository;
}
