using OSK.Hexagonal.MetaData;
using System;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// A builder that is used to configure the input system
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IInputSystemConfigurator
{
    IInputSystemConfigurator WithDeviceProvider<TDeviceProvider>()
        where TDeviceProvider : IDeviceProvider;

    /// <summary>
    /// Specifies the specific input scheme repository to use 
    /// </summary>
    /// <param name="type">The input scheme repository type that is desired to override the default memory repository</param>
    /// <returns>The builder for chaining</returns>
    IInputSystemConfigurator UseSchemeRepository(Type type);
}
