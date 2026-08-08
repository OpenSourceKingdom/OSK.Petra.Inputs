using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystemConfigurationProvider(InputSystemConfiguration configuration) : IInputSystemConfigurationProvider
{
    #region IInputSystemConfigurationProvider

    public InputSystemConfiguration Configuration => configuration;

    #endregion
}
