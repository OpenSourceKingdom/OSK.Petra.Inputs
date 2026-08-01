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
    /// The pairs of map input glyphs to actions
    /// </summary>
    IReadOnlyCollection<InputActionPair> InputMaps { get; }

    /// <summary>
    /// The list of unpaired actions that must be paired before saving
    /// </summary>
    IReadOnlyCollection<InputAction> UnpairedActions { get; }

    /// <summary>
    /// The list of unpaired inputs
    /// </summary>
    IReadOnlyCollection<IInput> UnpairedInputs { get; }

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
    /// <param name="action">The action to pair with the inputs</param>
    /// <param name="input">The input being mapped to the action</param>
    Output SetInputMap(InputAction action, IInput input);
}
