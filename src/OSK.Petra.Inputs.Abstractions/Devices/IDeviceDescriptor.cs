using System.Collections.Generic;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public interface IDeviceDescriptor
{
    DeviceIdentity Identity { get; }

    IReadOnlyCollection<IDeviceInput> Inputs { get; }

    IDeviceInput? GetInput(long id);
}
