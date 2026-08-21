using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

internal class TestDescriptor(DeviceIdentity identity) : IDeviceDescriptor
{
    public DeviceIdentity Identity => identity;

    public IReadOnlyCollection<IInput> Inputs => [];

    public IInput? GetInput(int id)
    {
        return null;
    }
}
