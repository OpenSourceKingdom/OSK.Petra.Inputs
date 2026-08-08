using OSK.Extensions.Petra.Inputs.Configuration.Options;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class ActionDefinitionBuilderExtensions
{
    extension(IActionDefinitionBuilder builder)
    {
        public IActionDefinitionBuilder WithAction(string name, IEnumerable<InputPhase> triggerPhases, Action<IInputEventContext> executor, InputActionOptions? options = null)
            => builder.WithAction(new InputAction(name, triggerPhases?.ToHashSet()!, executor, options?.Description, options?.ActionGroup));
    }
}
