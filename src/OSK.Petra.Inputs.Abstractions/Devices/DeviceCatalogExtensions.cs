namespace OSK.Petra.Inputs.Abstractions.Devices;

public static class DeviceCatalogExtensions
{
    extension(DeviceCatalog catalog)
    {
        public DevicePage? GetPage(DeviceIdentity deviceIdentity)
            => catalog.GetPage(deviceIdentity.TopologyName);
    }
}
