using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

internal class TestDescriptor(DeviceIdentity identity) : IDeviceDescriptor
{
    public DeviceIdentity Identity => identity;

    public IReadOnlyCollection<IInput> Inputs => [];

    public IInput? GetInput(long id)
    {
        return null;
    }
}
