using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public static class UserInputContextExtensions
{
    extension(IUserInputContext context)
    {
        /// <summary>
        /// Attempts to get the feature from the context, creating default feature information if it does not
        /// </summary>
        /// <typeparam name="TFeature">The type of feature to get</typeparam>
        /// <returns>The feature information</returns>
        public TFeature GetOrCreateFeature<TFeature>()
            where TFeature : ICapabilityFeature, new()
            => context.GetOrCreateFeature(() => new TFeature());

        /// <summary>
        /// Attempts to get the feature from the context, creating the feature information from the provided factory if it does not
        /// </summary>
        /// <typeparam name="TFeature">The type of feature to get</typeparam>
        /// <param name="factory">The factory to use if the feature does not currently exist on the context</param>
        /// <returns>The feature information</returns>
        /// <exception cref="ArgumentNullException">If the factory is null</exception>
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
