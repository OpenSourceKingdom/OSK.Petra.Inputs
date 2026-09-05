using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests._Helpers;

public class TestInput : IDeviceInput
{
    public long Id { get; }
    private readonly string _glyphSymbol;

    public TestInput(long id, string glyphSymbol = "X")
    {
        Id = id;
        _glyphSymbol = glyphSymbol;
    }

    public Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new InputGlyph
        {
            DeviceIdentity = default,
            Input = this,
            Text = _glyphSymbol
        }); 
}
