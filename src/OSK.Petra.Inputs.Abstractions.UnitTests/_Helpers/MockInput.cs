using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

public class MockInput : IInput
{
    public int Id { get; }
    private readonly string _glyphSymbol;

    public MockInput(int id, string glyphSymbol = "X")
    {
        Id = id;
        _glyphSymbol = glyphSymbol;
    }

    public InputGlyph GetGlyph() => new InputGlyph
    {
        DeviceIdentity = default,
        Input = this,
        Symbol = _glyphSymbol
    };
}
