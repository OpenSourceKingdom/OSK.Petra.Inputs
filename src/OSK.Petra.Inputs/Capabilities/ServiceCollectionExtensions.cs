using Microsoft.Extensions.DependencyInjection;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Petra.Inputs.Capabilities;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddStandardInputCapabilities()
        {
            services.AddTransient<IInputCapability, PowerCapability>();
            services.AddTransient<IInputCapability, PointerCapability>();

            return services;
        }
    }
}
