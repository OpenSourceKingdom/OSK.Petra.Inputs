using OSK.Hexagonal.MetaData;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// A builder that is used to configure the input system
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IInputSystemConfigurator
{
    /// <summary>
    /// Sets the configuration that will be used with the input system
    /// </summary>
    /// <param name="configuration">The configuration that will be used</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurator UseConfiguration(InputSystemConfiguration configuration);

    IInputSystemConfigurator WithDeviceProvider<TDeviceProvider>()
        where TDeviceProvider : IDeviceProvider;

    /// <summary>
    /// Specifies the specific input scheme repository to use 
    /// </summary>
    /// <param name="type">The input scheme repository type that is desired to override the default memory repository</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurator UseSchemeRepository(Type type);
}
