using Microsoft.Extensions.DependencyInjection;
using System;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Petra.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInputSystem(Action<IInputSystemBuilder> configurator)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            return services.AddInputs(c =>
            {
                var builder = new InputSystemBuilder();
                configurator(builder);

                if (builder.ScheemRepositoryType is not null)
                {
                    c.UseSchemeRepository(builder.ScheemRepositoryType);
                }
                c.UseConfiguration(builder.BuildConfiguration());

            });
        }
    }
}
