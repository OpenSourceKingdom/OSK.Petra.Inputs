using System;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public static class InputStateExtensions
{
    extension(IInputState state)
    {
        /// <summary>
        /// Determines if the state is being consumed by another input
        /// </summary>
        /// <returns>Whether the state is consumed</returns>
        public bool IsConsumed()
            => state.InputConsumer is not null;

        /// <summary>
        /// Attempts to get the details from the state and creating a default if it does not
        /// </summary>
        /// <typeparam name="TDetails">The type of input details to get</typeparam>
        /// <returns>The detail information</returns>
        public TDetails GetOrCreateDetails<TDetails>()
            where TDetails : ICapabilityDetails, new()
            => state.GetOrCreateDetails(() => new TDetails());

        /// <summary>
        /// Attempts to get the details from the state and using a factory to set the data if it does not
        /// </summary>
        /// <typeparam name="TDetails">The type of input details to get</typeparam>
        /// <param name="factory">The factory to use if the details do not exist in the state</param>
        /// <returns>The details information</returns>
        /// <exception cref="ArgumentNullException">If the factory is null</exception>
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
