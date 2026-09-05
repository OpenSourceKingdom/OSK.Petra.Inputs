using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

/// <summary>
/// Builder for creating an input scheme with device and virtual input mappings.
/// </summary>
public interface IInputSchemeBuilder
{
    /// <summary>
    /// Gets the name of the scheme being built.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Marks this scheme as the default scheme for its definition.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    IInputSchemeBuilder MakeDefault();

    /// <summary>
    /// Adds a device input mapping to an action.
    /// </summary>
    /// <param name="deviceIdentity">The device containing the input</param>
    /// <param name="inputId">The device input ID to map</param>
    /// <param name="actionName">The action name to map to</param>
    /// <returns>The builder for chaining</returns>
    IInputSchemeBuilder WithMap(DeviceIdentity deviceIdentity, long inputId, string actionName);

    /// <summary>
    /// Adds a virtual input mapping to an action.
    /// </summary>
    /// <param name="virtualInput">The virtual input to map</param>
    /// <param name="actionName">The action name to map to</param>
    /// <returns>The builder for chaining</returns>
    IInputSchemeBuilder WithVirtualInput(IVirtualInput virtualInput, string actionName);
}
