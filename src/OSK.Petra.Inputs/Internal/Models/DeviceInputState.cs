using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Services;

namespace OSK.Petra.Inputs.Internal.Models;

internal class DeviceInputState(DeviceInputContext context, IDeviceInput deviceInput): InputState(deviceInput)
{
    #region Variables

    private readonly InputOriginationSource _origination = new DeviceOriginationSource(context.DeviceIdentifier, deviceInput);

    public IDeviceInput DeviceInput => deviceInput;

    public RuntimeDeviceIdentifier DeviceIdentifier => context.DeviceIdentifier;

    #endregion

    #region InputState Overrides

    public override InputOriginationSource GetOriginationSource()
        => _origination;

    protected override void OnDispose()
    {
        context.RemoveState(this);
    }

    #endregion
}
