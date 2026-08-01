using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public interface IDeviceTopology
{
    DeviceTopologyName Name { get; }

    IReadOnlyCollection<IInput> Inputs { get; }
}
