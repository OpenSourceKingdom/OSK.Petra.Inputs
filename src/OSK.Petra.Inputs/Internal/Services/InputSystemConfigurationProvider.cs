using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Internal.Services;

internal partial class InputSystemConfigurationProvider: IInputSystemConfigurationProvider
{
    #region IInputSystemConfigurationProvider

    public required InputSystemConfiguration Configuration { get; set; }

    #endregion
}
