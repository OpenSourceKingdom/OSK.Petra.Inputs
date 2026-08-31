using System.Linq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public static class InputSystemConfigurationExtensions
{
    /// <summary>
    /// Attempts to get an input configuration that is the best fit for the device identity
    /// </summary>
    /// <param name="configuration">The input system configuration to search for a topology in</param>
    /// <param name="deviceIdentity">The identity of the device to get a topology for</param>
    /// <returns>The specific topology for the device identity if it is supported, otherwise null</returns>
    public static InputConfiguration? GetBestFitInputConfiguration(this InputSystemConfiguration configuration, DeviceIdentity deviceIdentity)
        => configuration.InputConfigurations.Select(configuration => new { Configuration = configuration, DeviceMatchStrength = configuration.GetDeviceSupportConfidence(deviceIdentity) })
                              .Where(configurationMatchData => configurationMatchData.DeviceMatchStrength > 0)
                              .OrderByDescending(configurationMatchData => configurationMatchData.DeviceMatchStrength)
                              .Select(configurationMatchData => configurationMatchData.Configuration)
                              .FirstOrDefault();
}
