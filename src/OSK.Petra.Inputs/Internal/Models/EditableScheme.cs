using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;

namespace OSK.Petra.Inputs.Internal.Models;

internal class EditableScheme: IEditableInputScheme
{
    #region Variables

    private string _schemeName;

    private readonly Dictionary<string, InputActionGlyphPair> _actionGlyphPairs;
    private readonly Dictionary<string, InputAction> _availableActionLookup;
    private readonly Dictionary<int, InputGlyph> _availableGlyphLookup;

    #endregion

    #region Constructors

    public EditableScheme(string name, bool isReadOnly, bool isPreferred, IEnumerable<InputAction> availableActions, IEnumerable<InputGlyph> availableInputs, IEnumerable<InputActionGlyphPair> paired)
    {
        _schemeName = name;
        IsReadonly = isReadOnly;
        IsPreferred = isPreferred;

        _actionGlyphPairs = paired.ToDictionary(pair => pair.Action.Name);
        _availableActionLookup = availableActions.ToDictionary(action => action.Name);
        _availableGlyphLookup = availableInputs.ToDictionary(glyph => glyph.Input.Id);

        foreach (var pair in paired)
        {
            _availableActionLookup.Remove(pair.Action.Name);
        }
    }

    #endregion

    #region IEditableInputScheme

    public bool IsReadonly { get; }

    public string Name
    {
        get => _schemeName;
        set
        {
            if (IsReadonly)
            {
                return;
            }

            _schemeName = value;
        }
    }

    public bool IsPreferred { get; set; }

    public IReadOnlyCollection<InputActionGlyphPair> ActionGlyphPairs => throw new NotImplementedException();

    public IReadOnlyCollection<InputAction> UnpairedActions => throw new NotImplementedException();

    public IReadOnlyCollection<InputGlyph> AvailableInputs => throw new NotImplementedException();

    public void SetActionGlyphPair(InputAction action, InputGlyph[] glyphs)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        if (glyphs is null)
        {
            throw new ArgumentNullException(nameof(glyphs));
        }
        if (glyphs.Length is 0)
        {
            throw new ArgumentException(nameof(glyphs), "Glyphs must contain at least one glyph");
        }

        var pairKey = GetActionGlyphPairKey(glyphs);

        // Deassociate any current pairs with the same input or action
        if (_actionGlyphPairs.TryGetValue(pairKey, out var currentPair))
        {
            _availableActionLookup[currentPair.Action.Name] = currentPair.Action;
            _actionGlyphPairs.Remove(action.Name);
        }

        _availableActionLookup.Remove(action.Name);
        _actionGlyphPairs[pairKey] = new InputActionGlyphPair(action, glyphs);
    }

    #endregion

    #region Helpers

    private string GetActionGlyphPairKey(IEnumerable<InputGlyph> glyphs)
        => string.Join(".", glyphs.Select(glyph => glyph.Symbol));

    #endregion
}
