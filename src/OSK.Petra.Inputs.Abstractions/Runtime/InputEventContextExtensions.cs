using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public static class InputEventContextExtensions
{
    extension(IInputEventContext context)
    {
        public bool TryGetDeviceFeature<TFeature>([NotNullWhen(true)] out TFeature? feature)
            where TFeature: ICapabilityFeature
        {
            feature = context.GetDeviceFeature<TFeature>();
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
