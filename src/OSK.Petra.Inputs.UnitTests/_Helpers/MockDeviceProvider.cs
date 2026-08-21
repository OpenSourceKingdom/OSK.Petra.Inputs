using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.UnitTests._Helpers;

public class MockDeviceProvider : IDeviceProvider
{
    public Task<Output<IEnumerable<IDeviceDescriptor>>> GetDevicesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Out.Success((IEnumerable<IDeviceDescriptor>)[]));
}
