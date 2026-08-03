using System.Collections.Generic;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public interface IDeviceDescriptor
{
    DeviceIdentity Identity { get; }

    IReadOnlyCollection<IInput> Inputs { get; }

    /// <summary>
    /// Determines whether this topology contains the specified input
    /// </summary>
    /// <param name="input">The input to check for</param>
    /// <returns>True if the input exists in this topology, otherwise false</returns>
    bool Contains(IInput input);
}
