using Microsoft.Extensions.Logging;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
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

    private readonly IEnumerable<IInputCapability> _capabilities;
    private readonly IInputSystemConfigurationProvider _configurationProvider;
    private readonly ISchemeService _schemeService;
    private readonly IUserManager _userManager;
    private readonly IInputSystemNotifier _systemNotifier;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InputService> _logger;

    private bool _globalActionSupression;
    private readonly Dictionary<int, UserInputContext> _userContexts = [];

    #endregion

    #region Constructors

    public InputService(IEnumerable<IInputCapability> capabilities, IInputSystemConfigurationProvider configurationProvider, IUserManager userManager, ISchemeService schemeService, 
        IInputSystemNotifier systemNotifier, IServiceProvider serviceProvider, ILogger<InputService> logger)
    {
        _capabilities = capabilities ?? throw new ArgumentNullException(nameof(capabilities));
        _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _systemNotifier = systemNotifier ?? throw new ArgumentNullException(nameof(systemNotifier));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (systemNotifier is null)
        {
            throw new ArgumentNullException(nameof(systemNotifier));
        }

        systemNotifier.OnDeviceNotification += ProcessDeviceNotification;
        systemNotifier.OnUserNotification += ProcessUserNotification;
        systemNotifier.OnSystemNotification += ProcessSystemNotification;
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
        foreach (var userDeviceContextPairs in _userContexts.Values.Select(userContext => userContext.DeviceInputContexts.Select(deviceContext => new { UserContext = userContext, DeviceContext = deviceContext })))
        {
            foreach (var data in userDeviceContextPairs.SelectMany(pair => pair.DeviceContext.GetInputStateSnapshot().Select(state => new { pair.UserContext, pair.DeviceContext, State = state })))
            {
                data.State.Duration = data.State.Duration.Add(delta);
                ProcessInput(data.UserContext, data.DeviceContext, data.State, delta);
            }
        }
    }

    #endregion

    #region Helpers

    internal bool IsGlobalInputSuppressed => _globalActionSupression;

    internal IEnumerable<UserInputContext> UserContexts => _userContexts.Values;

    /// <summary>
    /// An internal method to make testing easier to access and trigger
    /// </summary>
    /// <param name="notification">The notification to send for processing</param>
    internal void ProcessNotificationForTest(IInputSystemNotification notification)
    {
        switch (notification)
        {
            case DeviceInputNotification inputNotification:
                ProcessDeviceNotification(inputNotification);
                break;
            case UserNotification userNotification:
                ProcessUserNotification(userNotification);
                break;
            case SystemNotification systemNotification:
                ProcessSystemNotification(systemNotification);
                break;
        }
    }

    private void ProcessSystemNotification(SystemNotification systemNotification)
    {
        switch (systemNotification)
        {
            case ModifyActionGroupSuppressionNotification modifyActionGroupSuppressionNotification:
                var hasActionFilter = modifyActionGroupSuppressionNotification.ActionGroups is not null && modifyActionGroupSuppressionNotification.ActionGroups.Length > 0;
                var hasUserFilter = modifyActionGroupSuppressionNotification.UserIds is not null && modifyActionGroupSuppressionNotification.UserIds.Length > 0;

                // Global
                if (!hasActionFilter && !hasUserFilter)
                {
                    _globalActionSupression = modifyActionGroupSuppressionNotification.Suppress;
                    foreach (var userContext in _userContexts.Values)
                    {
                        userContext.Suppress(actionGroups: null, isSuppressed: _globalActionSupression);
                    }

                    return;
                }

                var userContexts = hasUserFilter
                    ? modifyActionGroupSuppressionNotification.UserIds.Where(id => _userContexts.TryGetValue(id, out _)).Select(id => _userContexts[id])
                    : _userContexts.Values;

                foreach (var useContext in userContexts)
                {
                    useContext.Suppress(actionGroups: modifyActionGroupSuppressionNotification.ActionGroups, modifyActionGroupSuppressionNotification.Suppress);
                }

                break;
            case SchemeEditorInputCaptureInitiatedNotification inputCaptureInitiatedNotification:
                if (_userContexts.TryGetValue(inputCaptureInitiatedNotification.UserId, out var initiatedUserContext))
                {
                    initiatedUserContext.EditorDelay = new SchemeEditorDelay()
                    {
                        Delay = inputCaptureInitiatedNotification.CaptureTimeout
                    };
                }
                break;
            case SchemeEditorInputCaptureTimeoutNotification inputCaptureTimeoutNotification:
                if (_userContexts.TryGetValue(inputCaptureTimeoutNotification.UserId, out var timeoutUserContext))
                {
                    timeoutUserContext.EditorDelay = null;
                }
                break;
        }
    }

    private void ProcessUserNotification(UserNotification userNotification)
    {
        switch (userNotification)
        {
            case UserRemovedNotification userRemovedNotification:
                _userContexts.Remove(userRemovedNotification.User.Id);
                break;
            case UserJoinedNotification userJoinedNotification:
                AddInputContext(userJoinedNotification.User);
                break;
        }
    }

    private void ProcessDeviceNotification(DeviceNotification deviceNotification)
    {
        if (deviceNotification is not DeviceInputNotification inputNotification)
        {
            return;
        }

        var configuration = _configurationProvider.Configuration;

        var topologyDescriptor = configuration.GetTopology(inputNotification.DeviceIdentifier.DeviceIdentity.TopologyName);
        if (topologyDescriptor is null)
        {
            LogUnsupportedInputDeviceInformation(_logger, inputNotification.DeviceIdentifier);
            _systemNotifier.Notify(new UnrecognizedDeviceNotification(inputNotification.DeviceIdentifier));
            return;
        }

        if (!topologyDescriptor.IsCompatibleInput(inputNotification.Input))
        {
            LogUnsupportedInputInformation(_logger, inputNotification.DeviceIdentifier, inputNotification.Input.GetGlyph().Symbol);
            _systemNotifier.Notify(new UnrecognizedDeviceInputNotification(inputNotification.DeviceIdentifier, inputNotification.Input));
            return;
        }

        var user = TryGetOrPairUserForDevice(configuration, inputNotification.DeviceIdentifier);
        if (user is null)
        {
            LogNoInputUserForDeviceWarning(_logger, deviceNotification.DeviceIdentifier);
            _systemNotifier.Notify(new UnpairedDeviceInputNotification(inputNotification.DeviceIdentifier, inputNotification.Input));
            return;
        }

        var setSchemeOutput = _schemeService.SetActiveSchemeForDevice(user.Id, inputNotification.DeviceIdentifier.DeviceIdentity);
        if (!setSchemeOutput.IsSuccessful)
        {
            return;
        }

        if (!_userContexts.TryGetValue(user.Id, out var userContext))
        {
            userContext = AddInputContext(user, setSchemeOutput.Data);
        }
         
        if (userContext.EditorDelay is not null)
        {
            _systemNotifier.Notify(new SchemeEditorInputCapturedNotification(userContext.UserId, inputNotification.DeviceIdentifier.DeviceIdentity, inputNotification.Input));
            return;
        }
        if (PauseInput)
        {
            return;
        }
        
        if (setSchemeOutput.StatusCode.Status == OutputStatus.Updated)
        {
            userContext.Scheme = setSchemeOutput.Data;
        }

        var deviceContext = userContext.GetOrAddDevice(deviceNotification.DeviceIdentifier);
        var inputState = deviceContext.GetOrCreateState(inputNotification.Input);

        ProcessInput(userContext, deviceContext, inputState, inputNotification.DeltaTime);
    }

    private void ProcessInput(UserInputContext userContext, DeviceInputContext deviceContext, InputState inputState, TimeSpan deltaTime)
    {
        foreach (var capability in _capabilities.Where(capability => capability.CanProces(inputState.Input)))
        {
            capability.Process(deviceContext, inputState, deltaTime);
            if (inputState.IsDisposed)
            {
                return;
            }
        }

        if (!inputState.IsNewActivation && inputState.Phase is InputPhase.End)
        {
            return;
        }

        inputState.Reset();

        var inputMap = userContext.Scheme?.GetInputMap(deviceContext.DeviceIdentifier.DeviceIdentity, inputState.Input.Id);
        if (inputMap is null)
        {
            return;
        }

        var action = inputMap.Value.Action;
        if (action is null || (action.ActionGroup.HasValue && !userContext.IsSuppressed(action.ActionGroup.Value)))
        {
            return;
        }

        if (action.TriggerPhases.Contains(inputState.Phase))
        {
            action.ActionExecutor(new InputEventContext(userContext.UserId, deviceContext.DeviceIdentifier, deviceContext, inputState, deltaTime, _serviceProvider));
            LogInputActionTriggeredDebug(_logger, userContext.UserId, deviceContext.DeviceIdentifier, inputState.Input.GetGlyph().Symbol, action.Name);
            _systemNotifier.Notify(new ActionExecutedNotification(userContext.UserId, userContext.Scheme.DefinitionName, action.Name));
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

        var inputConfiguration = _configurationProvider.Configuration.GetBestFitInputConfiguration(deviceIdentifier.DeviceIdentity);
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

    private UserInputContext AddInputContext(IInputUser user, InputScheme? scheme = null)
    {
        var userContext = new UserInputContext(user.Id)
        {
            Scheme = scheme
        };
        _userContexts[user.Id] = userContext;

        if (_globalActionSupression)
        {
            userContext.Suppress(null, true);
        }

        return userContext;
    }

    #endregion
}
