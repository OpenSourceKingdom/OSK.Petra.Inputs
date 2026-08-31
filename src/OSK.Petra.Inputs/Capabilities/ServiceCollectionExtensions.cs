using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Ports;

namespace OSK.Petra.Inputs.Capabilities;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddStandardInputCapabilities()
        {
            services.TryAddTransient<ICapabilityOptionsProvider, CapabilityOptionsProvider>();
            services.TryAddTransient(typeof(ICapabilityOptions<>), typeof(CapabilityOptionsWrapper<>));

            services.AddTransient<IInputCapability, PowerCapability>();
            services.AddTransient<IInputCapability, PointerCapability>();

            return services;
        }
    }
}
