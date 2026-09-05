using OSK.Petra.Inputs.Abstractions.Devices;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Capabilities.Power;

/// <summary>
/// A virtual input that combines multiple device inputs 
/// </summary>
public interface IPowerCombinationInput: IVirtualInput
{
    /// <summary>
    /// Gets the collection of device input identifiers that compose this combination input.
    /// </summary>
    IReadOnlyCollection<DeviceInputIdentifier> InputIdentifiers { get; }
}
