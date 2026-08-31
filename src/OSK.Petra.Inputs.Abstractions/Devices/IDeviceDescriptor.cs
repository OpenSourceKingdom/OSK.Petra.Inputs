using System.Collections.Generic;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public interface IDeviceDescriptor
{
    DeviceIdentity Identity { get; }

    IReadOnlyCollection<IInput> Inputs { get; }

    IInput? GetInput(long id);
}
