using OSK.Petra.Inputs.Abstractions.Configuration;
using System;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

/// <summary>
/// Builder for creating an action definition with actions and schemes.
/// </summary>
public interface IActionDefinitionBuilder
{
    /// <summary>
    /// Marks this definition as the default definition.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    IActionDefinitionBuilder MakeDefault();

    /// <summary>
    /// Adds an action to the definition.
    /// </summary>
    /// <param name="action">The action to add</param>
    /// <returns>The builder for chaining</returns>
    IActionDefinitionBuilder WithAction(InputAction action);

    /// <summary>
    /// Adds an input scheme to the definition.
    /// </summary>
    /// <param name="name">The scheme name</param>
    /// <param name="configurator">Configuration callback for building the scheme</param>
    /// <returns>The builder for chaining</returns>
    IActionDefinitionBuilder WithScheme(string name, Action<IInputSchemeBuilder> configurator);
}
