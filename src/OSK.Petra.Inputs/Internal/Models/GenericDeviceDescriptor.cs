using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Internal.Models;

internal readonly struct GenericDeviceDescriptor(IDeviceTopology deviceTopology, DeviceFamily family) : IDeviceDescriptor
{
    #region IDeviceDescriptor

    public DeviceIdentity DeviceIdentity { get; } = new DeviceIdentity(deviceTopology.Name, family, "Generic");

    public IEnumerable<IInput> GetInputs()
        => deviceTopology.Inputs;

    public bool Contains(IInput input)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
