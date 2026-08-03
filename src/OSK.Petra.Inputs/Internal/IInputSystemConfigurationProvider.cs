using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Internal;

internal interface IInputSystemConfigurationProvider
{
    InputSystemConfiguration Configuration { get; set; }
}
