using System;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Extensions.Petra.Inputs.Configuration;

/// <summary>
/// Factory for creating input system configurations using the builder pattern.
/// </summary>
public static class InputSystemConfigurationFactory
{
    /// <summary>
    /// Creates an input system configuration using the provided configuration action.
    /// </summary>
    /// <param name="configurator">Action that configures the builder</param>
    /// <returns>The built input system configuration</returns>
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
