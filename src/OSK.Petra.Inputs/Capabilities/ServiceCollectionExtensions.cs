using Microsoft.Extensions.DependencyInjection;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Petra.Inputs.Capabilities;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        IServiceCollection AddStatndardInputCapabilities()
        {
            services.AddTransient<IInputCapability, PowerCapability>();
            services.AddTransient<IInputCapability, PointerCapability>();

            return services;
        }
    }
}
