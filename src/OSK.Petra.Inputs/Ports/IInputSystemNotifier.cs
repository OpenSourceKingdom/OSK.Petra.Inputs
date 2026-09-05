using System;
using OSK.Hexagonal.MetaData;
using OSK.Petra.Inputs.Notifications;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// A notifier that transmits notifications relating to various device, user, or other input system events
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IInputSystemNotifier
{
    /// <summary>
    /// Action event for device notifications
    /// </summary>
    event Action<DeviceNotification> OnDeviceNotification;

    /// <summary>
    /// Action event for input user notifications
    /// </summary>
    event Action<UserNotification> OnUserNotification;

    /// <summary>
    /// Action event for input system notifications
    /// </summary>
    event Action<SystemNotification> OnSystemNotification;

    /// <summary>
    /// Transmits a notification through the input system notifier.
    /// </summary>
    /// <param name="inputNotification">The notification to transmit</param>
    void Notify(IInputSystemNotification inputNotification);
}
