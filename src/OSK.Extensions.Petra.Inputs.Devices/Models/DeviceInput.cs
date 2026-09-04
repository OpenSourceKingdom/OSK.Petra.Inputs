using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class DeviceInput(long id): IDeviceInput
{
    public long Id => id;

    public abstract Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default);
}
