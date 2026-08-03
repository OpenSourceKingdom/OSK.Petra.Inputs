using System.Collections.Generic;
using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Models;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface ISelectedScheme
{
    /// <summary>
    /// The scheme can not be edited, apart from read-only safe properties, like <see cref="IsPreferred"/>
    /// </summary>
    bool IsReadonly { get; }

    /// <summary>
    /// Sets the name for the input scheme
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether the scheme is preferred
    /// </summary>
    bool IsPreferred { get; }

    /// <summary>
    /// Whether the scheme is a new scheme being created, that is it isn't actually saved yet
    /// </summary>
    bool IsNew { get; }

    /// <summary>
    /// The pairs of map input glyphs to actions
    /// </summary>
    IReadOnlyList<DeviceMapPairing<InputActionMap>> ConfiguredInputMaps { get; }

    /// <summary>
    /// The list of unpaired inputs
    /// </summary>
    IReadOnlyList<DeviceMapPairing<IInput>> UnpairedInputs { get; }

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
    /// Sets the current scheme as the preferred scheme
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
    Output SetInputMap(DeviceIdentity deviceIdentity, InputAction action, IInput input);
}
