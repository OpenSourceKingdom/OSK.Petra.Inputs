using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Internal.Services;

internal class CapabilityOptionsProvider(IInputSystemConfigurationProvider configurationProvider) : ICapabilityOptionsProvider
{
    #region ICapabilityOptionsProvider

    public TCapabilityOptions Get<TCapabilityOptions>()
        where TCapabilityOptions : CapabilityOptions, new()
        => configurationProvider.Configuration.CapabilityConfiguration.TryGetOptions<TCapabilityOptions>(out var options)
            ? options
            : new();

    #endregion
}
