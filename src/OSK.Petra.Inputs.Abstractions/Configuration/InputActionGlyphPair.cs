using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public class InputActionGlyphPair(InputAction action, InputGlyph[] glyphs)
{
    public InputAction Action => action;

    public InputGlyph[] Glyphs => glyphs;
}
