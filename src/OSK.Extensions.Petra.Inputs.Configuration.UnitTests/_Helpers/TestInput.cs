using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests._Helpers;

public class TestInput : IInput
{
    public long Id { get; }
    private readonly string _glyphSymbol;

    public TestInput(long id, string glyphSymbol = "X")
    {
        Id = id;
        _glyphSymbol = glyphSymbol;
    }

    public InputGlyph GetGlyph() => new InputGlyph
    {
        DeviceIdentity = default,
        Input = this,
        Text = _glyphSymbol
    };
}
