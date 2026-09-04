using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public class DeviceOriginationSource(RuntimeDeviceIdentifier deviceIdentifier, IDeviceInput deviceInput): InputOriginationSource
{
    #region Api

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    public IDeviceInput Input => deviceInput;

    #endregion
}
