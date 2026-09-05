using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

/// <summary>
/// Represents an input that is associated with an actual device
/// </summary>
/// <param name="id">The id for the input on the device</param>
public abstract class DeviceInput(long id): IDeviceInput
{
    public long Id => id;

    public abstract Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default);
}
