using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.UnitTests._Helpers;

public class MockInput : IDeviceInput
{
    public long Id { get; }
    private readonly string _glyphSymbol;

    public MockInput(long id, string glyphSymbol = "X")
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
