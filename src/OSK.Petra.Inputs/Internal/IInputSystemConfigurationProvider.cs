using OSK.Hexagonal.MetaData;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Internal;

[HexagonalIntegration(HexagonalIntegrationType.ConsumerRequired)]
internal interface IInputSystemConfigurationProvider
{
    InputSystemConfiguration Configuration { get; set; }
}
