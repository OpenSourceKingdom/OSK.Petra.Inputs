using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Ports;
using System;

namespace OSK.Petra.Inputs;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the core services for the input system and processing to the service collection, using default configuration
    /// </summary>
    /// <param name="services">The services to add the DI to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInputSystem(this IServiceCollection services)
        => services.AddInputSystem(_ => { });

    /// <summary>
    /// Adds the core services for the input system and processing to the service collection.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>If no scheme repository is specified with the input system builder, a default in-memory one will be utilized</item>
    /// </list>
    /// </remarks>
    /// <param name="services">The services to add the DI to</param>
    /// <param name="configurator">The action to configure the input system</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input system configuration build configurator is null</exception>
    public static IServiceCollection AddInputSystem(this IServiceCollection services, Action<IInputSystemBuilder> configurator)
    {
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        services.TryAddSingleton<IDeviceDescriptorProvider, DeviceDescriptorProvider>();
        services.TryAddSingleton<ISchemeService, SchemeService>();
        services.TryAddSingleton<IInputService, InputService>();
        services.TryAddSingleton<IUserManager, UserManager>();
        services.TryAddSingleton<IInputSystem, InputSystem>();
        services.TryAddSingleton<IInputSystemNotifier, InputSystemNotifier>();
        services.TryAddSingleton<IInputConfigurationProvider, InputConfigurationProvider>();

       // var builder = new InputSystemBuilder(services);
       // configurator(builder);
       // builder.UseSchemeRepository<InMemorySchemeRepository>();

        return services;
    }
}
