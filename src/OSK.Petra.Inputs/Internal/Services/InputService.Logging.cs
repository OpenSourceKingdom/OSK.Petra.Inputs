using Microsoft.Extensions.Logging;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Internal.Services;

internal partial class InputService
{
    #region Logging

    [LoggerMessage(eventId: 1, LogLevel.Warning, "An input was received for a device named '{deviceIdentifier}' but no user was found to be paired to it, thus the input will be ignored.")]
    private static partial void LogNoInputUserForDeviceWarning(ILogger logger, RuntimeDeviceIdentifier deviceIdentifier);

    [LoggerMessage(eventId: 2, LogLevel.Information, "An input device, '{deviceIdentifier}', sent input but was not paird due to the policy's manual device handling")]
    private static partial void LogUnpairdDeviceDueToPolicyInformation(ILogger logger, RuntimeDeviceIdentifier deviceIdentifier);

    [LoggerMessage(eventId: 3, LogLevel.Information, "An input device, '{deviceIdentifier}, sent input but was not paired because it was not part of a supported input configuration.")]
    private static partial void LogUnpairedDeviceDueToUnsupportedConfigurationInformation(ILogger logger, RuntimeDeviceIdentifier deviceIdentifier);

    [LoggerMessage(eventId: 4, LogLevel.Debug, "A new input device, '{deviceIdentifier}', sent input and no user was found to possess it, new user being created due to policy's settings.")]
    private static partial void LogNewUserCreatedDebug(ILogger logger, RuntimeDeviceIdentifier deviceIdentifier);

    [LoggerMessage(eventId: 5, LogLevel.Debug, "Input processing pause state changed, tracking input: {pause}")]
    private static partial void LogTogglePauseDebug(ILogger logger, bool pause);

    [LoggerMessage(eventId: 6, LogLevel.Information, "Input was received for input device '{deviceIdentifier}' but it is not a supported input device, ignoring input processing.")]
    private static partial void LogUnsupportedInputDeviceInformation(ILogger logger, RuntimeDeviceIdentifier deviceIdentifier);

    [LoggerMessage(eventId: 7, LogLevel.Information, "Input was received for input device '{deviceIdentifier}' but the input '{inputSymbol}' is not supported for the device, ignoring input processing.")]
    private static partial void LogUnsupportedInputInformation(ILogger logger, RuntimeDeviceIdentifier deviceIdentifier, string inputSymbol);

    [LoggerMessage(eventId: 8, LogLevel.Debug, "Input received, from device {deviceIdentifier}, for user {userId} has triggered an action '{actionName}' for input scheme {activeScheme}.")]
    private static partial void LogInputActionTriggeredDebug(ILogger logger, int userId, RuntimeDeviceIdentifier deviceIdentifier, string activeScheme, string actionName);

    [LoggerMessage(eventId: 9, LogLevel.Warning, "An attempt was made to pair device {deviceIdentifier} to user {userId} but it failed.")]
    private static partial void LogDevicePairingFailedWarning(ILogger logger, int userId, RuntimeDeviceIdentifier deviceIdentifier);

    #endregion
}
