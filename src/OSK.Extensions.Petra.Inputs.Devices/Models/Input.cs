using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class Input(long id): IInput
{
    public long Id => id;

    public abstract InputGlyph GetGlyph();
}
