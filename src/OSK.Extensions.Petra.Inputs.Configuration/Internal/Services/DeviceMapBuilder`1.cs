using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class DeviceMapBuilder<TInput>(DeviceIdentity identity) : DeviceMapBuilder(identity), IDeviceMapBuilder<TInput>
    where TInput : IInput
{
    #region IDeviceMapBuilder

    public IDeviceMapBuilder<TInput> WithMap(TInput input, string actionName)
    {
        WithMap((IInput)input, actionName);
        return this;
    }

    #endregion
}
