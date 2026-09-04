using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public static class InputEventContextExtensions
{
    extension(IInputEventContext context)
    {
        public bool TryGetDeviceFeature<TFeature>([NotNullWhen(true)] out TFeature? feature)
            where TFeature: ICapabilityFeature
        {
            feature = context.GetInputFeature<TFeature>();
            return feature is not null;
        }

        public bool TryGetInputDetails<TDetails>([NotNullWhen(true)] out TDetails? details)
            where TDetails: ICapabilityDetails
        {
            details = context.GetInputDetails<TDetails>();
            return details is not null;
        }
    }
}
