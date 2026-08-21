using System.Collections.Generic;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public interface IDeviceDescriptor
{
    DeviceIdentity Identity { get; }

    IReadOnlyCollection<IInput> Inputs { get; }

    IInput? GetInput(int id);
}
