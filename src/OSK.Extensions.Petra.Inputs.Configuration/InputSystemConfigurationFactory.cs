using System;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class InputSystemConfigurationFactory
{
    public static InputSystemConfiguration Create(Action<IInputSystemConfigurationBuilder> configurator)
    {
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        var builder = new InputSystemConfigurationBuilder();
        configurator(builder);

        return builder.BuildConfiguration();
    }
}
