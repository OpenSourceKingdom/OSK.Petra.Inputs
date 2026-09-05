using OSK.Petra.Inputs.Abstractions.Runtime;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Options;

/// <summary>
/// Provides configuration options when a user joins the input system.
/// </summary>
public class UserJoinOptions
{
    /// <summary>
    /// The input devices to automatically pair to the user upon creation.
    /// </summary>
    public IEnumerable<RuntimeDeviceIdentifier>? DevicesToPair { get; set; }

    /// <summary>
    /// The preferred active action definition name to assign to the user upon creation.
    /// </summary>
    public string? ActiveDefinitionName { get; set; }
}
