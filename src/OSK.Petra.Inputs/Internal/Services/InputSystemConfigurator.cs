using OSK.Petra.Inputs.Abstractions;
using System;
using OSK.Petra.Inputs.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using OSK.Petra.Inputs.Capabilities;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystemConfigurator : IInputSystemConfigurator
{
    #region Variables

    private HashSet<Type> _deviceProviderTypes = [];
    private Type? _schemeRepositoryType;

    #endregion

    #region IInputSystemConfigurator

    public IInputSystemConfigurator UseSchemeRepository(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (!typeof(ISchemeRepository).IsAssignableFrom(type))
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
        _schemeRepositoryType ??= typeof(InMemorySchemeRepository);

        services.TryAddSingleton(typeof(ISchemeRepository), _schemeRepositoryType);
        foreach (var deviceProviderType in _deviceProviderTypes)
        {
            services.AddTransient(typeof(IDeviceProvider), deviceProviderType);
        }

        services.AddSingleton<IInputSystemConfigurationProvider, InputSystemConfigurationProvider>();

        services.TryAddSingleton<IInternalSchemeService, SchemeService>();
        services.TryAddSingleton<ISchemeService>(sp => sp.GetRequiredService<IInternalSchemeService>());
        services.TryAddSingleton<IInputService, InputService>();
        services.TryAddSingleton<IUserManager, UserManager>();
        services.TryAddSingleton<IInputSystem, InputSystem>();
        services.TryAddSingleton<IInputSystemNotifier, InputSystemNotifier>();
        services.TryAddSingleton<IDeviceCatalogProvider, DeviceCatalogProvider>();

        services.AddStandardInputCapabilities();
    }

    #endregion
}
