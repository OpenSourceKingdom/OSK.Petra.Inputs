using OSK.Petra.Inputs.Abstractions;
using System;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystemConfigurator : IInputSystemConfigurator
{
    #region Variables

    private InputSystemConfiguration? _configuration;
    private HashSet<Type> _deviceProviderTypes = [];
    private Type? _schemeRepositoryType;

    #endregion

    #region IInputSystemConfigurator

    public IInputSystemConfigurator UseConfiguration(InputSystemConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var validation = InputSystemConfigurationValidator.ValidateConfiguration(configuration);
        if (!validation.IsValid)
        {
            throw new InputSystemValidationException(validation);
        }

        _configuration = configuration;

        return this;
    }

    public IInputSystemConfigurator UseSchemeRepository(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (!type.IsAssignableFrom(typeof(ISchemeRepository)))
        {
            throw new InvalidOperationException($"The repositry type '{type.FullName}' does not match the expected type of an '{typeof(ISchemeRepository).FullName}'");
        }

        _schemeRepositoryType = type;
        return this;
    }

    public IInputSystemConfigurator WithDeviceProvider<TDeviceProvider>() 
        where TDeviceProvider : IDeviceProvider
    {
        _deviceProviderTypes.Add(typeof(TDeviceProvider));

        return this;
    }

    #endregion

    #region Helpers

    internal void ConfigureServices(IServiceCollection services)
    {
        if (_configuration is null)
        {
            throw new ArgumentNullException(nameof(_configuration));
        }

        _schemeRepositoryType ??= typeof(InMemorySchemeRepository);

        services.TryAddSingleton(typeof(ISchemeRepository), _schemeRepositoryType);
        foreach (var deviceProviderType in _deviceProviderTypes)
        {
            services.TryAddTransient(typeof(IDeviceProvider), deviceProviderType);
        }

        services.AddSingleton<IInputSystemConfigurationProvider>(_ => new InputSystemConfigurationProvider(_configuration));

        services.TryAddSingleton<IInternalSchemeService, SchemeService>();
        services.TryAddSingleton<ISchemeService>(sp => sp.GetRequiredService<IInternalSchemeService>());
        services.TryAddSingleton<IInputService, InputService>();
        services.TryAddSingleton<IUserManager, UserManager>();
        services.TryAddSingleton<IInputSystem, InputSystem>();
        services.TryAddSingleton<IInputSystemNotifier, InputSystemNotifier>();
        services.TryAddSingleton<IDeviceCatalogProvider, DeviceCatalogProvider>();
    }

    #endregion
}
