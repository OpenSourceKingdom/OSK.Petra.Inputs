using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public static class DeviceInputContextExtensions
{
    extension(IDeviceInputContext context)
    {
        public TFeature GetOrCreateFeature<TFeature>()
            where TFeature : ICapabilityFeature, new()
            => context.GetOrCreateFeature(() => new TFeature());

        public TFeature GetOrCreateFeature<TFeature>(Func<TFeature> factory)
            where TFeature : ICapabilityFeature
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var Feature = context.GetFeature<TFeature>();
            if (Feature is null)
            {
                Feature = factory();
                context.SetFeature(Feature);
            }

            return Feature;
        }

    }
}
