using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class InputSchemeBuilderExtensions
{
    extension(IInputSchemeBuilder schemeBuilder)
    {
        /// <summary>
        /// Adds a strongly typed input map to a scheme builder
        /// </summary>
        /// <typeparam name="TInput">The type of input being added</typeparam>
        /// <param name="deviceIdentity">The device that the input belongs to</param>
        /// <param name="input">The input to add</param>
        /// <param name="actionName">The action name that the input maps to</param>
        /// <returns>The scheme builder for chaining</returns>
        public IInputSchemeBuilder WithMap<TInput>(DeviceIdentity deviceIdentity, TInput input, string actionName)
            where TInput : IDeviceInput
            => schemeBuilder.WithMap(deviceIdentity, input.Id, actionName);
    }
}
