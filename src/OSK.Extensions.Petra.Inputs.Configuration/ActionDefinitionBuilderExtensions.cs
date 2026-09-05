using OSK.Extensions.Petra.Inputs.Configuration.Options;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class ActionDefinitionBuilderExtensions
{
    extension(IActionDefinitionBuilder builder)
    {
        /// <summary>
        /// Adds an action to the action definition builder
        /// </summary>
        /// <param name="name">The name of the action</param>
        /// <param name="triggerPhases">The expected trigger phases for the input</param>
        /// <param name="executor">The action to execute once an input event is fired that satisfies the conditions to run</param>
        /// <param name="options">Optional parameters for the specific action</param>
        /// <returns>The builder for chaining</returns>
        public IActionDefinitionBuilder WithAction(string name, IEnumerable<InputPhase> triggerPhases, Action<IInputEventContext> executor, InputActionOptions? options = null)
            => builder.WithAction(new InputAction(name, triggerPhases?.ToHashSet()!, executor, options?.Description, options?.ActionGroup));
    }
}
