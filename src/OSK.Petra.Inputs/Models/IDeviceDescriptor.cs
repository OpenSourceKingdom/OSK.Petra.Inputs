using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Models;

public interface IDeviceDescriptor
{
    DeviceIdentity DeviceIdentity { get; }

    IEnumerable<IInput> GetInputs();

    bool Contains(IInput input);
}
