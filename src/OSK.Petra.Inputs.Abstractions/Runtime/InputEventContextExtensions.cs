using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public static class InputEventContextExtensions
{
    extension(IInputEventContext context)
    {
        /// <summary>
        /// Attempts to get the input feature from the context
        /// </summary>
        /// <typeparam name="TFeature">The type of input feature to get</typeparam>
        /// <param name="feature">The feature information, if it exists</param>
        /// <returns>Whether the feature existed</returns>
        public bool TryGetInputFeature<TFeature>([NotNullWhen(true)] out TFeature? feature)
            where TFeature: ICapabilityFeature
        {
            feature = context.GetInputFeature<TFeature>();
            return feature is not null;
        }

        /// <summary>
        /// Attempts to get the input details from the context
        /// </summary>
        /// <typeparam name="TDetails">The type of details to get</typeparam>
        /// <param name="details">The detail information, if it exists</param>
        /// <returns>Whether the details existed</returns>
        public bool TryGetInputDetails<TDetails>([NotNullWhen(true)] out TDetails? details)
            where TDetails: ICapabilityDetails
        {
            details = context.GetInputDetails<TDetails>();
            return details is not null;
        }
    }
}
