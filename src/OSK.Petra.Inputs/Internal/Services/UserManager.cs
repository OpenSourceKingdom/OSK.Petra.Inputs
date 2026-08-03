using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSK.Operations.Outputs.Models;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions;
using Microsoft.Extensions.Logging;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Options;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Internal.Services;

internal partial class UserManager: IUserManager
{
    #region Variables

    private readonly IInputSystemConfigurationProvider _configurationProvider;
    private readonly IInputSystemNotifier _systemNotifier;
    private readonly ISchemeRepository _schemeRepository;
    private readonly ILogger<UserManager> _logger;

    private readonly Dictionary<int, InputUser> _users = [];

    #endregion

    #region Constructors

    public UserManager(IInputSystemConfigurationProvider configurationProvider, IInputSystemNotifier systemNotifier, ISchemeRepository schemeRepository, ILogger<UserManager> logger)
    {
        _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        _schemeRepository = schemeRepository ?? throw new ArgumentNullException(nameof(schemeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _systemNotifier = systemNotifier ?? throw new ArgumentNullException(nameof(systemNotifier));
    }

    #endregion

    #region IInputUserManager

    public Output<IInputUser> CreateUser(UserJoinOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (_configurationProvider.Configuration.JoinPolicy.MaxUsers <= _users.Count)
        {
            return Out.Error<IInputUser>(OutputStatus.InvalidRange, $"Unable to create user as the maximum number of users ({_configurationProvider.Configuration.JoinPolicy.MaxUsers}) has been reached.");
        }

        var devicesToPair = options.DevicesToPair ?? [];
        var alreadyPairedDevices = devicesToPair.Where(device => GetUserForDevice(device.DeviceId) is not null);
        if (alreadyPairedDevices.Any())
        {
            var inputDeviceError = string.Join(",", alreadyPairedDevices.Select(device => device.DeviceIdentity));
            return Out.InvalidRequest<IInputUser>($"Unable to create user as one more devices have already been paired to the input system: {inputDeviceError}");
        }

        var newUserId = _users.Count is 0
            ? 1
            : _users.Values.Max(user => user.Id) + 1;

        ActionDefinition? inputDefinition = null;
        var useActiveDefinition = !string.IsNullOrWhiteSpace(options.ActiveDefinitionName);
        if (useActiveDefinition)
        {
            inputDefinition = _configurationProvider.Configuration.Definitions.FirstOrDefault(definition
                => definition.Name.Equals(options.ActiveDefinitionName, StringComparison.OrdinalIgnoreCase));
        }

        inputDefinition = inputDefinition ?? _configurationProvider.Configuration.Definitions.FirstOrDefault(definition => definition.IsDefault)
            ?? _configurationProvider.Configuration.Definitions.First();

        if (useActiveDefinition && !inputDefinition.Name.Equals(options.ActiveDefinitionName, StringComparison.OrdinalIgnoreCase))
        {
            LogCreateUseActiveDefinitionNameNotFoundWarning(_logger, options.ActiveDefinitionName!, inputDefinition.Name);
        }

        _users[newUserId] = new InputUser(newUserId)
        {
            ActiveDefinitionName = inputDefinition.Name
        };

        _systemNotifier.Notify(new UserJoinedNotification(_users[newUserId]));

        foreach (var device in devicesToPair)
        {
            _users[newUserId].AddDevice(device);
            _systemNotifier.Notify(new DevicePairedNotification(newUserId, device));
        }

        return Out.Success((IInputUser)_users[newUserId]);
    }

    public Output SetActiveDefinition(int userId, string definitionName)
    {
        if (!_users.TryGetValue(userId, out var user))
        {
            LogSetActiveDefinitionForBadUserInformation(_logger, userId);
            return Out.DataNotFound($"User {userId} does not exist.");
        }

        if (string.IsNullOrWhiteSpace(definitionName))
        {
            LogActiveDefinitionNameNotFoundWarning(_logger, "{Empty}");
            return Out.InvalidRequest("Input definition name cannot be null.");
        }

        var definition = _configurationProvider.Configuration.GetDefinition(definitionName);
        if (definition is null)
        {
            LogActiveDefinitionNameNotFoundWarning(_logger, definitionName);
            return Out.DataNotFound($"Input definition with name {definition} does not exist.");
        }

        user.ActiveDefinitionName = definitionName;

        _systemNotifier.Notify(new UserActiveDefinitionChangeNotification(user, user.ActiveDefinitionName));
        return Out.Success();
    }

    public IInputUser? GetUserForDevice(int deviceId)
        => _users.Values.FirstOrDefault(user => user.GetPairedDevice(deviceId) is not null);

    public IInputUser? GetUser(int userId)
        => _users.TryGetValue(userId, out var user)
            ? user
            : null;

    public IEnumerable<IInputUser> GetUsers()
        => _users.Values;

    public bool RemoveUser(int userId)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            foreach (var deviceIdentifier in user.GetPairedDevices().Select(device => device.DeviceIdentifier).ToArray())
            {
                UnpairDevice(userId, deviceIdentifier.DeviceId);
            }

            _users.Remove(userId);
            _systemNotifier.Notify(new UserRemovedNotification(user));
            return true;
        }

        return false;
    }

    public Output PairDevice(int userId, RuntimeDeviceIdentifier device)
    {
        if (!_users.TryGetValue(userId, out var user))
        {
            return Out.DataNotFound($"Unable to pair device {device.DeviceId}, {device.DeviceIdentity.Name}, because there is no user with id {userId}.");
        }

        var pairedUser = GetUserForDevice(device.DeviceId);
        if (pairedUser is not null)
        {
            return pairedUser.Id == userId
                ? Out.Success()
                : Out.InvalidRequest($"Unable to pair device {device.DeviceId}, {device.DeviceIdentity.Name}, to user {userId} because it is already paired to {pairedUser.Id}.");
        }

        user.AddDevice(device);
        return Out.Success();
    }

    public bool UnpairDevice(int userId, int deviceId)
    {
        if (!_users.TryGetValue(userId, out var user))
        {
            return false;
        }

        var pairedDevice = user.RemoveDevice(deviceId);
        if (pairedDevice is not null)
        {
            _systemNotifier.Notify(new DeviceUnpairedNotification(userId, pairedDevice.DeviceIdentifier));
            return true;
        }

        return false;
    }

    public async Task<Output> SavePreferredSchemeAsync(PreferredInputScheme scheme, CancellationToken cancellationToken = default)
    {
        if (scheme.UserId < 0 || scheme.UserId >= _configurationProvider.Configuration.JoinPolicy.MaxUsers)
        {
            return Out.InvalidRequest($"The provided user id must be non-zero and less than the max users ({_configurationProvider.Configuration.JoinPolicy.MaxUsers}) for the input system.");
        }

        if (string.IsNullOrWhiteSpace(scheme.DefinitionName))
        {
            return Out.InvalidRequest("Definition name can not be empty.");
        }

        var definition = _configurationProvider.Configuration.GetDefinition(scheme.DefinitionName);
        if (definition is null)
        {
            return Out.DataNotFound($"No input definition with the name '{scheme.DefinitionName}' exists.");
        }

        if (string.IsNullOrWhiteSpace(scheme.SchemeName))
        {
            return Out.InvalidRequest("Scheme name can not be empty.");
        }


        if (string.IsNullOrWhiteSpace(scheme.ConfigurationId))
        {
            return Out.InvalidRequest("Configuration Id can not be empty.");
        }

        var inputConfiguration = _configurationProvider.Configuration.GetInputConfiguration(scheme.ConfigurationId);
        if (inputConfiguration is null)
        {
            return Out.DataNotFound($"No input configuration for '{scheme.ConfigurationId}' exists.");
        }

        if (inputConfiguration.GetScheme(scheme.DefinitionName, scheme.SchemeName) is null)
        {
            return Out.DataNotFound($"No input scheme named '{scheme.SchemeName}' exists on the definition '{scheme.DefinitionName}' for the input configuration '{scheme.ConfigurationId}'");
        }

        // Fix scheme not taking effect
        return await _schemeRepository.SavePreferredSchemeAsync(scheme, cancellationToken);
    }

    #endregion

    #region Logging

    [LoggerMessage(eventId: 1, LogLevel.Warning, "An attempt was made to create a user with a specified active definition name '{definitionName}' that was not found in the input system, defaulting to using '{defaultDefinitionName}'.")]
    private static partial void LogCreateUseActiveDefinitionNameNotFoundWarning(ILogger logger, string definitionName, string defaultDefinitionName);

    [LoggerMessage(eventId: 2, LogLevel.Warning, "An attempt was made to create a user for the definition name '{definitionName}' with the active input scheme '{schemeName}' but the scheme was not found, defaulting to using '{defaultSchemeName}'.")]
    private static partial void LogCreateUserActiveSchemeNameNotFoundWarning(ILogger logger,  string definitionName, string schemeName, string defaultSchemeName);

    [LoggerMessage(eventId: 3, LogLevel.Warning, "An attempt was made set a user with a specified active definition name '{definitionName}' that was not found in the input system, ignoring.")]
    private static partial void LogActiveDefinitionNameNotFoundWarning(ILogger logger, string definitionName);

    [LoggerMessage(eventId: 4, LogLevel.Information, "An attempt was made to set a user input scheme for the definition name '{definitionName}' with the active input scheme '{schemeName}' but the scheme was not found, ignoring.")]
    private static partial void LogActiveSchemeNameNotFoundInformation(ILogger logger, string definitionName, string schemeName);

    [LoggerMessage(eventId: 5, LogLevel.Information, "An attempt was made to set the active definition for user {userId} but that user does not exist, ignoring.")]
    private static partial void LogSetActiveDefinitionForBadUserInformation(ILogger logger, int userId);

    #endregion
}
