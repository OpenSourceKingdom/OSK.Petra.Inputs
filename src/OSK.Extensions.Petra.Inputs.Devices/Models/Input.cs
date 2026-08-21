using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class Input(int id): IInput
{
    public int Id => id;

    public abstract InputGlyph GetGlyph();
}
