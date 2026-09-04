using OSK.Petra.Inputs.Abstractions.Devices;
using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public static class InputStateExtensions
{
    extension(IInputState state)
    {
        public bool IsConsumed()
            => state.InputConsumer is not null;

        public TDetails GetOrCreateDetails<TDetails>()
            where TDetails : ICapabilityDetails, new()
            => state.GetOrCreateDetails(() => new TDetails());

        public TDetails GetOrCreateDetails<TDetails>(Func<TDetails> factory)
            where TDetails : ICapabilityDetails
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var details = state.GetDetails<TDetails>();
            if (details is null)
            {
                details = factory();
                state.SetDetails(details);
            }

            return details;
        }

    }
}
