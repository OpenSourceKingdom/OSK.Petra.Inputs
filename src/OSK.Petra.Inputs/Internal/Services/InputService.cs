using Microsoft.Extensions.Logging;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Options;
using OSK.Petra.Inputs.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Internal.Services;

internal partial class InputService : IInputService
{
    #region Variables

    private bool _paused;

    private readonly IInputCapability[] _capabilities;
    private readonly IInputConfigurationProvider _configurationProvider;
    private readonly ISchemeService _schemeService;
    private readonly IUserManager _userManager;
    private readonly IDeviceDescriptorProvider _deviceDescriptorProvider;
    private readonly IInputSystemNotifier _systemNotifier;
    private readonly ILogger<InputService> _logger;

    private readonly Dictionary<int, UserInputContext> _userContexts = [];

    #endregion

    #region Constructors

    public InputService(IEnumerable<IInputCapability> capabilities, IInputConfigurationProvider configurationProvider, IUserManager userManager, ISchemeService schemeService, IDeviceDescriptorProvider deviceDescriptorProvider, IInputSystemNotifier systemNotifier,
        ILogger<InputService> logger)
    {
        _capabilities = capabilities?.ToArray() ?? throw new ArgumentNullException(nameof(capabilities));
        _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _deviceDescriptorProvider = deviceDescriptorProvider ?? throw new ArgumentNullException(nameof(deviceDescriptorProvider));
        _systemNotifier = systemNotifier ?? throw new ArgumentNullException(nameof(systemNotifier));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (systemNotifier is null)
        {
            throw new ArgumentNullException(nameof(systemNotifier));
        }

        systemNotifier.OnDeviceNotification += ProcessDeviceNotification;
    }

    #endregion

    #region IInputService

    public bool PauseInput 
    {
        get => _paused;
        set
        {
            _paused = value;
            LogTogglePauseDebug(_logger, _paused);
        }
    }

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

        var deviceDescriptor = _deviceDescriptorProvider.GetDescriptorForDevice(inputNotification.DeviceIdentifier.DeviceIdentity);
        if (deviceDescriptor is null)
        {
            LogUnsupportedInputDeviceInformation(_logger, inputNotification.DeviceIdentifier);
            _systemNotifier.Notify(new UnrecognizedDeviceNotification(inputNotification.DeviceIdentifier));
            return;
        }

        if (!deviceDescriptor.Contains(inputNotification.Input))
        {
            LogUnsupportedInputInformation(_logger, inputNotification.DeviceIdentifier, inputNotification.Input.GetGlyph().Symbol);
            _systemNotifier.Notify(new UnrecognizedDeviceNotification(inputNotification.DeviceIdentifier));
            return;
        }

        var user = TryGetOrPairUserForDevice(configuration, inputNotification.DeviceIdentifier);
        if (user is null)
        {
            LogNoInputUserForDeviceWarning(_logger, deviceNotification.DeviceIdentifier);
            _systemNotifier.Notify(new UnrecognizedDeviceNotification(inputNotification.DeviceIdentifier));
            return;
        }

        if (_userContexts.TryGetValue(user.Id, out var userContext))
        {
            userContext.DeviceIdentifier = inputNotification.DeviceIdentifier;
            userContext.Input = inputNotification.Input;
        }
        else
        {
            userContext = new UserInputContext(user.Id)
            {
                DeviceIdentifier = inputNotification.DeviceIdentifier,
                Input = inputNotification.Input
            };
            _userContexts[user.Id] = userContext;
        }

        foreach (var capability in _capabilities)
        {
            if (capability.CanProces(userContext.Input))
            {
                capability.Process(userContext);
            }
        }

        var scheme = _schemeService.GetActiveSchemeForUser(user.Id);
        if (scheme is null)
        {
            return;
        }
        
        var deviceMap = scheme.GetDeviceMap(inputNotification.DeviceIdentifier.DeviceIdentity);
        if (deviceMap is null)
        {
            return;
        } 
    }

    private IInputUser? TryGetOrPairUserForDevice(InputSystemConfiguration configuration, RuntimeDeviceIdentifier deviceIdentifier)
    {
        var user = _userManager.GetUserForDevice(deviceIdentifier.DeviceId);
        if (user is not null)
        {
            return user;
        }
        if (configuration.JoinPolicy.DeviceJoinBehavior is DevicePairingBehavior.Manual)
        {
            LogUnpairdDeviceDueToPolicyInformation(_logger, deviceIdentifier);
            return null;
        }

        var inputConfiguration = _configurationProvider.Configuration.GetBestInputConfiguration(deviceIdentifier.DeviceIdentity);
        if (inputConfiguration is null)
        {
            LogUnpairedDeviceDueToUnsupportedConfigurationInformation(_logger, deviceIdentifier);
            return null;
        }

        var supportedConfigurations = configuration.InputConfigurations;
        var userDevicePairingData = _userManager.GetUsers().Select(user =>
        {
            var pairedDeviceSet = user.PairedDevices.Select(pairedDevice => pairedDevice.DeviceIdentifier.DeviceIdentity.TopologyName).ToHashSet();
            var completedConfigurations = supportedConfigurations.Count(configuration => configuration.TopologyNames.All(identity => pairedDeviceSet.Contains(identity)));
            var missingNewDevice = !pairedDeviceSet.Contains(deviceIdentifier.DeviceIdentity.TopologyName);
            var fewestDevicesToCompleteClosestCombinationWithDevice = missingNewDevice
                ? supportedConfigurations
                    .Where(configuration => configuration.TopologyNames.Contains(deviceIdentifier.DeviceIdentity.TopologyName))
                    .Select(configuration => configuration.TopologyNames.Count(identity => !pairedDeviceSet.Contains(identity)))
                    .Min()
                : 100;

            return new DevicePairingDetails(user, missingNewDevice, user.PairedDevices.Count, completedConfigurations, fewestDevicesToCompleteClosestCombinationWithDevice);
        }).ToArray();

        if (configuration.JoinPolicy.UserJoinBehavior is not UserJoinBehavior.Manual && configuration.JoinPolicy.MaxUsers > userDevicePairingData.Length
            && userDevicePairingData.All(pairingData => pairingData.TotalCompletedConfigurations >= 1))
        {
            LogNewUserCreatedDebug(_logger, deviceIdentifier);
            var createUserOutput = _userManager.CreateUser(new UserJoinOptions() { DevicesToPair = [deviceIdentifier] });
            return createUserOutput.IsSuccessful
                ? createUserOutput.Data
                : null;
        }

        switch (configuration.JoinPolicy.DeviceJoinBehavior)
        {
            case DevicePairingBehavior.Balanced:
                var pairingUser = userDevicePairingData.OrderByDescending(pairingData => pairingData.MissingDevice)
                                                  .ThenBy(pairingData => pairingData.MinimumDevicesToCompleteCombination)
                                                  .ThenBy(pairingData => pairingData.TotalPairedDevices)
                                                  .ThenBy(pairingData => pairingData.TotalCompletedConfigurations)
                                                  .FirstOrDefault().User;
                var pairedDeviceOutput = _userManager.PairDevice(pairingUser.Id, deviceIdentifier);
                if (pairedDeviceOutput.IsSuccessful)
                {
                    return pairingUser;
                }

                LogDevicePairingFailedWarning(_logger, pairingUser.Id, deviceIdentifier);
                _systemNotifier.Notify(new DevicePairingFailedNotification(pairingUser.Id, deviceIdentifier));
                return pairingUser;
            default:
                return null;
        }
    }

    #endregion
}
