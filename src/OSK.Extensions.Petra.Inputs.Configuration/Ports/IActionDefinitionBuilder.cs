using OSK.Petra.Inputs.Abstractions.Configuration;
using System;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IActionDefinitionBuilder
{
    IActionDefinitionBuilder MakeDefault();

    IActionDefinitionBuilder WithAction(InputAction action);

    IActionDefinitionBuilder WithScheme(string name, Action<IInputSchemeBuilder> configurator);
}
