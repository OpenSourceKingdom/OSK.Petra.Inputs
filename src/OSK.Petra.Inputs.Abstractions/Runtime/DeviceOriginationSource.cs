using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// Describes the origination source for a particular input within an <see cref="IInputEventContext"/> that originated from a device
/// </summary>
/// <param name="deviceIdentifier">The device that contains the input</param>
/// <param name="deviceInput">The input that triggered the event</param>
public class DeviceOriginationSource(RuntimeDeviceIdentifier deviceIdentifier, IDeviceInput deviceInput): InputOriginationSource
{
    #region Api

    /// <summary>
    /// The device that owns the input
    /// </summary>
    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    /// <summary>
    /// The input that triggered the event
    /// </summary>
    public IDeviceInput Input => deviceInput;

    #endregion
}
