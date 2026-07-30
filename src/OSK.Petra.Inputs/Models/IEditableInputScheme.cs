using System.Collections.Generic;
using OSK.Hexagonal.MetaData;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Models;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IEditableInputScheme
{
    /// <summary>
    /// The scheme can not be edited, apart from read-only safe properties, like <see cref="IsPreferred"/>
    /// </summary>
    bool IsReadonly { get; }

    /// <summary>
    /// Sets the name for the input scheme
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Sets whether the scheme is preferred
    /// </summary>
    bool IsPreferred { get; set; }

    /// <summary>
    /// The pairs of map input glyphs to actions
    /// </summary>
    IReadOnlyCollection<InputMap> InputMaps { get; }

    /// <summary>
    /// The list of unpaired actions that must be paired before saving
    /// </summary>
    IReadOnlyCollection<InputAction> UnpairedActions { get; }

    /// <summary>
    /// The list of available inputs
    /// </summary>
    IReadOnlyCollection<InputGlyph> AvailableInputs { get; }

    /// <summary>
    /// Sets an action glyph pair
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Pairs will be replaced if they match another known pair</item>
    /// </list>
    /// </remarks>
    /// <param name="action"></param>
    /// <param name="glyphs"></param>
    void SetActionGlyphPair(InputAction action, InputGlyph[] glyphs);
}
