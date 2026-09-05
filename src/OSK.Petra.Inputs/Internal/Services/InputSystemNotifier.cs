using System;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystemNotifier : IInputSystemNotifier
{
    #region IInputSystemNotificationPublisher

    public event Action<DeviceNotification> OnDeviceNotification = delegate { };
    public event Action<UserNotification> OnUserNotification = delegate { };
    public event Action<SystemNotification> OnSystemNotification = delegate { };

    public void Notify(IInputSystemNotification notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        switch (notification)
        {
            case DeviceNotification deviceNotification:
                OnDeviceNotification(deviceNotification);
                break;
            case UserNotification userNotification:
                OnUserNotification(userNotification);
                break;            
            case SystemNotification systemNotification:
                OnSystemNotification(systemNotification);
                break;
            default:
                throw new InvalidOperationException($"The notifier was not configured to publish an event of type '{notification.GetType().FullName}'.");
        }
    }

    #endregion
}
