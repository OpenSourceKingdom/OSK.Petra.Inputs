using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests._Helpers;

public class TestInput : IInput
{
    public int Id { get; }
    private readonly string _glyphSymbol;

    public TestInput(int id, string glyphSymbol = "X")
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
