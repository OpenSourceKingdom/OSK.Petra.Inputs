using System.Collections.Generic;
using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Ports;

namespace OSK.Petra.Inputs.Models;

/// <summary>
/// Represents a scheme that is selected by an <see cref="ISchemeEditor"/>
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface ISelectedScheme
{
    /// <summary>
    /// Describes if the scheme can be edited, apart from read-only safe properties, like <see cref="IsPreferred"/>
    /// </summary>
    bool IsReadonly { get; }

    /// <summary>
    /// The name for the input scheme
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether the scheme is preferred by the user
    /// </summary>
    bool IsPreferred { get; }

    /// <summary>
    /// Whether the scheme is a new scheme being created, i.e. isn't actually saved yet
    /// </summary>
    bool IsNew { get; }

    /// <summary>
    /// The pairs of device action maps
    /// </summary>
    IReadOnlyList<DeviceMapPairing<DeviceInputActionMap>> ConfiguredInputMaps { get; }

    /// <summary>
    /// The list of unpaired inputs
    /// </summary>
    IReadOnlyList<DeviceMapPairing<IDeviceInput>> UnpairedInputs { get; }

    /// <summary>
    /// The list of unpaired actions that must be paired before saving
    /// </summary>
    IReadOnlyList<InputAction> UnpairedActions { get; }

    /// <summary>
    /// Sets the name of the scheme
    /// </summary>
    /// <param name="name">The new name</param>
    /// <returns>Whether the operation succeeded</returns>
    Output SetName(string name);

    /// <summary>
    /// Marks the current scheme as the preferred scheme for the user.
    /// </summary>
    void MakePreferred();

    /// <summary>
    /// Sets an input map with the provided inputs and action
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Input maps will be replaced if they match another known map</item>
    /// </list>
    /// </remarks>
    /// <param name="deviceIdentity">The device that is being given a map</param>
    /// <param name="action">The action to pair with the inputs</param>
    /// <param name="input">The input being mapped to the action</param>
    Output SetInputMap(DeviceIdentity deviceIdentity, InputAction action, IDeviceInput input);

    /// <summary>
    /// Clears all configured input maps from the scheme, i.e. resets the scheme to its original state
    /// </summary>
    /// <returns>
    /// An output describing whether the operation succeeded
    /// </returns>
    Output ClearConfiguredMaps();
}
