using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Internal;

internal interface IInputConfigurationProvider
{
    InputSystemConfiguration Configuration { get; set; }
}
