using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class InputSchemeBuilderExtensions
{
    extension(IInputSchemeBuilder schemeBuilder)
    {
        public IInputSchemeBuilder WithMap<TInput>(DeviceIdentity deviceIdentity, TInput input, string actionName)
            where TInput : IInput
            => schemeBuilder.WithMap(deviceIdentity, input, actionName);
    }
}
