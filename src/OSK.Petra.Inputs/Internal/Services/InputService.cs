using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputService : IInputService
{
    #region Variables

    private readonly IInputCapability[] _capabilities;
    private readonly IInputConfigurationProvider _configurationProvider;
    private readonly IUserManager _userManager;

    #endregion

    #region Constructors

    public InputService(IEnumerable<IInputCapability> capabilities, IInputConfigurationProvider configurationProvider, IUserManager userManager, IInputSystemNotifier notifier)
    {
        _capabilities = capabilities?.ToArray() ?? throw new ArgumentNullException(nameof(capabilities));
        _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        if (notifier is null)
        {
            throw new ArgumentNullException(nameof(notifier));
        }

        notifier.OnDeviceNotification += ProcessDeviceNotification;
    }

    #endregion

    #region IInputService

    public bool PauseInput { get; set; }

    public void Update(TimeSpan delta)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Helpers

    private void ProcessDeviceNotification(DeviceNotification deviceNotification)
    {
        if (deviceNotification is not DeviceInputNotification inputNotification)
        {
            return;
        }

        var configuration = _configurationProvider.Configuration;

        var user = _userManager.GetUserForDevice(inputNotification.DeviceIdentifier.DeviceId);
        if (user is null)
        {
            return;
        }

        var deviceTopology = configuration.GetDeviceTopology(inputNotification.DeviceIdentifier.DeviceIdentity);
        if (deviceTopology is null)
        {
            return;
        }

        var context = new InputProcessingContext(inputNotification.DeviceIdentifier, inputNotification.Input);
        foreach (var capability in _capabilities)
        {
            if (capability.CanProces(context.Input))
            {
                capability.Process(context);
            }
        }


    }

    private IInputUser? GetOrCreateUserForDevice(InputSystemConfiguration configuration, RuntimeDeviceIdentifier deviceIdentifier)
    {
        var user = _userManager.GetUserForDevice(deviceIdentifier.DeviceId);
        if (user is not null)
        {
            return user;
        }

        if (configuration.JoinPolicy.UserJoinBehavior is UserJoinBehavior.Manual)
        {
            return null;
        }
        if (configuration.JoinPolicy.DeviceJoinBehavior is DevicePairingBehavior.Manual)
        {
            return null;
        }
    }

    #endregion
}
