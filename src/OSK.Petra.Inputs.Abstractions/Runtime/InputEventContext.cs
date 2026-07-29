using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public class InputEventContext(int userId, TimeSpan deltaTime, RuntimeDeviceIdentifier deviceIdentifier, IInput input, IEnumerable<CapabilityData> capabilityData, IServiceProvider serviceProvider)
{
    #region Variables

    private readonly Dictionary<Type, CapabilityData> _capabilitiesLookup = capabilityData?.ToDictionary(data => data.GetType()) ?? [];

    #endregion

    #region Api

    /// <summary>
    /// The user who initiated the event
    /// </summary>
    public int UserId => userId;

    /// <summary>
    /// The amount of time that has occurred since the last frame was processed
    /// </summary>
    public TimeSpan DeltaTime => deltaTime;

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    public IInput Input => input;

    /// <summary>
    /// The services associated with this event context
    /// </summary>
    public IServiceProvider Services => serviceProvider;

    #endregion

    #region Helpers

    #endregion
}
