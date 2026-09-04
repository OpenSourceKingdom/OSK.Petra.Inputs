using OSK.Petra.Inputs.Abstractions.Devices;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Capabilities.Power;

public interface IPowerCombinationInput: IVirtualInput
{
    IReadOnlyCollection<DeviceInputIdentifier> InputIdentifiers { get; }
}
