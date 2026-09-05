namespace OSK.Petra.Inputs.Abstractions.Devices;

public static class DeviceCatalogExtensions
{
    extension(DeviceCatalog catalog)
    {
        /// <summary>
        /// Attempts to get a device page using the device directly
        /// </summary>
        /// <param name="deviceIdentity">The device to get the page for</param>
        /// <returns>The device page if it exists</returns>
        public DevicePage? GetPage(DeviceIdentity deviceIdentity)
            => catalog.GetPage(deviceIdentity.TopologyName);
    }
}
