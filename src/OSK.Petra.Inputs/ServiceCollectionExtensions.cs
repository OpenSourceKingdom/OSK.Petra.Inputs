using Microsoft.Extensions.DependencyInjection;
using OSK.Petra.Inputs.Exceptions;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Ports;
using System;

namespace OSK.Petra.Inputs;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the core services for the input system and processing to the service collection.
        /// </summary>
        /// <remarks>
        /// 💡 Notes:
        /// <list type="bullet">
        /// <item>If no scheme repository is specified with the input system builder, a default in-memory one will be utilized.</item>
        /// </list>
        /// </remarks>
        /// <param name="configurator">The action to configure the input system.</param>
        /// <returns>The service collection for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the input system configuration builder configurator is null.
        /// </exception>
        /// <exception cref="InputSystemValidationException">If the provided configuration is not valid</exception>
        public IServiceCollection AddInputSystem(Action<IInputSystemConfigurator> configurator)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            var systemConfigurator = new InputSystemConfigurator();
            configurator(systemConfigurator);
            systemConfigurator.ConfigureServices(services);

            return services;
        }
    }
}
 