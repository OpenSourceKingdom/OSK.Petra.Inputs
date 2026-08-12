using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Internal.Services;

internal partial class SchemeService(IInputSystemConfigurationProvider configurationProvider, ISchemeRepository schemeRepository, IUserManager userManager, IInputSystemNotifier systemNotifier,
    IDeviceCatalogProvider deviceCatalogProvider, IServiceProvider serviceProvider, ILogger logger): IInternalSchemeService
{
    #region Variables

    /// <summary>
    /// A cache data storage for the input schemes. It is ordered by:
    /// - Input Configuration Id
    /// - Action Definition Id
    /// - Scheme Name
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, InputScheme>>> _customSchemeLookup = [];

    /// <summary>
    /// A cache data storage for user preferences. It is ordered by:
    /// - User Id
    /// - Input Configuration Id
    /// - Definition Name
    /// </summary>
    private readonly Dictionary<int, Dictionary<string, Dictionary<string, PreferredInputScheme>>> _userPreferredSchemesLookup = [];

    private readonly Dictionary<int, InputScheme> _activeUserSchemesLookup = [];

    #endregion

    #region ISchemeService

    public bool AllowCustomSchemes => schemeRepository.AllowCustomSchemes;

    public PreferredInputScheme? GetPreferredInputScheme(int userId, string inputConfigurationId, string definitionName)
        => !string.IsNullOrWhiteSpace(inputConfigurationId) && !string.IsNullOrWhiteSpace(definitionName)
            && _userPreferredSchemesLookup.TryGetValue(userId, out var inputConfigurationSchemeLookup) && inputConfigurationSchemeLookup.TryGetValue(inputConfigurationId, out var definitionSchemeLookup)
            && definitionSchemeLookup.TryGetValue(definitionName, out var scheme)
            ? scheme
            : null;

    public IEnumerable<InputScheme> GetInputSchemes(string inputConfigurationId, string definitionName)
        => (configurationProvider.Configuration.GetInputConfiguration(inputConfigurationId)?.Schemes.Where(scheme => scheme.DefinitionName.Equals(definitionName, StringComparison.OrdinalIgnoreCase)) ?? [])
            .Concat(GetCustomInputSchemes(inputConfigurationId, definitionName));

    public async Task<Output<ISchemeEditor>> GetSchemeEditorAsync(int userId, CancellationToken cancellation = default)
    {
        var user = userManager.GetUser(userId);
        if (user is null)
        {
            return Out.DataNotFound<ISchemeEditor>($"No user was found for id {userId}");
        }

        var getCatalogOutput = await deviceCatalogProvider.GetCatalogAsync(cancellation);
        if (!getCatalogOutput.IsSuccessful)
        {
            return getCatalogOutput.As<ISchemeEditor>();
        }

        var editor = ActivatorUtilities.CreateInstance<SchemeEditor>(serviceProvider, user, getCatalogOutput.Data);

        return Out.Success((ISchemeEditor)editor);
    }

    public InputScheme? GetActiveSchemeForUser(int userId)
    {
        var user = userManager.GetUser(userId);
        if (user is null)
        {
            return null;
        }

        return _activeUserSchemesLookup.TryGetValue(userId, out var activeScheme)
            ? activeScheme
            : null;
    }

    public Output<InputScheme> SetActiveSchemeForDevice(int userId, DeviceIdentity deviceIdentity)
    {
        var user = userManager.GetUser(userId);
        if (user is null)
        {
            return Out.DataNotFound<InputScheme>($"No user with the id {userId} exists.");
        }
        if (_activeUserSchemesLookup.TryGetValue(userId, out var activeScheme) && activeScheme.ContainsTopology(deviceIdentity.TopologyName))
        {
            return Out.Success(activeScheme);
        }

        var inputConfiguration = configurationProvider.Configuration.GetBestFitInputConfiguration(deviceIdentity);
        if (inputConfiguration is null)
        {
            return Out.InvalidRequest<InputScheme>($"No input configuration exists that supports the device identity {deviceIdentity}.");
        }

        var definition = configurationProvider.Configuration.GetDefinition(user.ActiveDefinitionName) ?? configurationProvider.Configuration.Definitions.First(definition => definition.IsDefault);
        activeScheme = GetActiveScheme(userId, definition, inputConfiguration, deviceIdentity);
        _activeUserSchemesLookup[userId] = activeScheme;

        if (logger.IsEnabled(LogLevel.Information))
        {
            LogNewActiveSchemeInformation(logger, user.Id, inputConfiguration.GetDisplayName(), definition.Name, activeScheme.Name);
        }

        systemNotifier.Notify(new UserActiveSchemeChangeNotification(user, inputConfiguration, definition.Name, activeScheme.Name));

        return Out.Updated(activeScheme);
    }

    public async Task<Output> SavePreferredSchemeAsync(PreferredInputScheme scheme, CancellationToken cancellationToken = default)
    {
        if (scheme.UserId < 0 || scheme.UserId >= configurationProvider.Configuration.JoinPolicy.MaxUsers)
        {
            return Out.InvalidRequest($"The provided user id must be non-zero and less than the max users ({configurationProvider.Configuration.JoinPolicy.MaxUsers}) for the input system.");
        }

        if (string.IsNullOrWhiteSpace(scheme.DefinitionName))
        {
            return Out.InvalidRequest("Definition name can not be empty.");
        }

        var definition = configurationProvider.Configuration.GetDefinition(scheme.DefinitionName);
        if (definition is null)
        {
            return Out.DataNotFound($"No input definition with the name '{scheme.DefinitionName}' exists.");
        }

        if (string.IsNullOrWhiteSpace(scheme.SchemeName))
        {
            return Out.InvalidRequest("Scheme name can not be empty.");
        }

        if (configurationProvider.Configuration.GetInputConfiguration(scheme.ConfigurationId) is null)
        {
            return Out.DataNotFound($"No input scheme named '{scheme.SchemeName}' exists for the definition '{scheme.DefinitionName}' for the input  '{scheme.ConfigurationId}'");
        }

        // Fix scheme not taking effect
        return await schemeRepository.SavePreferredSchemeAsync(scheme, cancellationToken);
    }

    public async Task<Output> SaveCustomSchemeAsync(CustomInputScheme scheme, SchemeSavePermissions savePermissions, CancellationToken cancellationToken = default)
    {
        if (scheme is null)
        {
            throw new ArgumentNullException(nameof(scheme));
        }

        if (!AllowCustomSchemes)
        {
            return Out.InvalidRequest("Custom input schemes are not allowed with the input system. If it is desired, please register a scheme repository that can support it.");
        }

        var schemeValidation = InputSystemConfigurationValidator.ValidateCustomScheme(configurationProvider.Configuration, scheme, allowDuplicateCustomScheme: savePermissions is SchemeSavePermissions.Overwrite);
        if (!schemeValidation.IsValid)
        {
            return schemeValidation.Result is InputConfigurationValidation.DuplicateData
                ? Out.DuplicateData($"The scheme name {scheme.Name} already exists on input definition {scheme.DefinitionName}, if overwriting is desired then ensure the save flag is set correctly.")
                : Out.InvalidRequest($"There was a validation error with the custom scheme: {Environment.NewLine}{schemeValidation.Message}");
        }

        var saveOutput = await schemeRepository.SaveCustomInputScheme(scheme, cancellationToken);
        if (!saveOutput.IsSuccessful)
        {
            return saveOutput;
        }

        var configurationId = InputConfiguration.GetConfigurationId(scheme.GetDeviceIdentities());
        if (!_customSchemeLookup.TryGetValue(configurationId, out var configurationSchemeLookup))
        {
            configurationSchemeLookup = [];
            _customSchemeLookup[configurationId] = configurationSchemeLookup;
        }
        if (!configurationSchemeLookup.TryGetValue(scheme.DefinitionName, out var definitionSchemeLookup))
        {
            definitionSchemeLookup = [];
            configurationSchemeLookup[scheme.DefinitionName] = definitionSchemeLookup;
        }

        definitionSchemeLookup[scheme.Name] = scheme.ToInputScheme();
        return Out.Success();
    }

    public async Task<Output> DeleteCustomSchemeAsync(string definitionName, string schemeName, CancellationToken cancellationToken = default)
    {
        if (!AllowCustomSchemes
            || string.IsNullOrWhiteSpace(definitionName)
            || string.IsNullOrWhiteSpace(schemeName)
            || configurationProvider.Configuration.GetDefinition(definitionName) is null)
        {
            return Out.Success();
        }

        return await schemeRepository.DeleteCustomSchemeAsync(definitionName, schemeName, cancellationToken);
    }

    public async Task<Output> LoadSchemeConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _customSchemeLookup.Clear();
        _userPreferredSchemesLookup.Clear();

        var getUserPreferredSchemes = await schemeRepository.GetPreferredSchemesAsync(cancellationToken);
        if (getUserPreferredSchemes.IsSuccessful)
        {
            // Only take one preferred scheme for each definition, even if the repository returns multiples.
            // There should only ever be 1 preferred scheme per definition, so multiples would indicate either a
            // mistake in the repository or some malicious intent
            foreach (var userPreferredScheme in getUserPreferredSchemes.Data.Where(preferredScheme =>
            {
                if (preferredScheme.UserId < 0 || preferredScheme.UserId > configurationProvider.Configuration.JoinPolicy.MaxUsers)
                {
                    return false;
                }

                var inputConfiguration = configurationProvider.Configuration.GetInputConfiguration(preferredScheme.ConfigurationId);
                if (inputConfiguration is null)
                {
                    return false;
                }

                return inputConfiguration.GetScheme(preferredScheme.DefinitionName, preferredScheme.SchemeName) is not null;
            }))
            {
                if (!_userPreferredSchemesLookup.TryGetValue(userPreferredScheme.UserId, out var inputConfigurationSchemeLookup))
                {
                    inputConfigurationSchemeLookup = [];
                    _userPreferredSchemesLookup[userPreferredScheme.UserId] = inputConfigurationSchemeLookup;
                }
                if (!inputConfigurationSchemeLookup.TryGetValue(userPreferredScheme.ConfigurationId, out var definitionSchemeLookup))
                {
                    definitionSchemeLookup = [];
                    inputConfigurationSchemeLookup[userPreferredScheme.ConfigurationId] = definitionSchemeLookup;
                }

                definitionSchemeLookup[userPreferredScheme.DefinitionName] = userPreferredScheme;
            }
        }
        else
        {
            LogLoadActiveInputFailedWarning(logger, getUserPreferredSchemes.GetErrorString());
        }

        if (schemeRepository.AllowCustomSchemes)
        {
            var getCustomSchemesOutput = await schemeRepository.GetCustomSchemesAsync(cancellationToken);
            if (getCustomSchemesOutput.IsSuccessful)
            {
                foreach (var customScheme in getCustomSchemesOutput.Data)
                {
                    var configurationId = InputConfiguration.GetConfigurationId(customScheme.GetDeviceIdentities());
                    if (!_customSchemeLookup.TryGetValue(configurationId, out var configurationSchemeLookup))
                    {
                        configurationSchemeLookup = new();
                        _customSchemeLookup[configurationId] = configurationSchemeLookup;
                    }
                    if (!configurationSchemeLookup.TryGetValue(customScheme.DefinitionName, out var definitionSchemeLookup))
                    {
                        definitionSchemeLookup = new();
                        configurationSchemeLookup[customScheme.DefinitionName] = definitionSchemeLookup;
                    }

                    definitionSchemeLookup[customScheme.Name] = customScheme.ToInputScheme();
                }
            }
            else
            {
                LogLoadCustomSchemesFailedWarning(logger, getCustomSchemesOutput.GetErrorString());
            }
        }

        return Out.Success();
    }

    #endregion

    #region Helpers

    private IEnumerable<InputScheme> GetCustomInputSchemes(string inputConfigurationId, string definitionName)
        => _customSchemeLookup.TryGetValue(inputConfigurationId, out var definitionSchemeLookup) && definitionSchemeLookup.TryGetValue(definitionName, out var schemeLookup)
            ? schemeLookup.Values
            : [];

    private InputScheme GetActiveScheme(int userId, ActionDefinition definition, InputConfiguration inputConfiguration, DeviceIdentity deviceIdentity)
    {
        var preferredScheme = GetPreferredInputScheme(userId, inputConfiguration.Id, definition.Name);

        InputScheme? activeScheme = null;
        if (preferredScheme is not null)
        {
            activeScheme = inputConfiguration.Schemes.Concat(GetCustomInputSchemes(inputConfiguration.Id, definition.Name)).FirstOrDefault(scheme => scheme.Name.Equals(preferredScheme.Value.SchemeName));
        }

        activeScheme ??= inputConfiguration.Schemes.First(scheme => scheme.IsDefault);
        return activeScheme;
    }

    #endregion

    #region Logging

    [LoggerMessage(eventId: 1, LogLevel.Warning, "An error was encountered when attempting to get active input schemes, using an empty list; error: {error}")]
    private static partial void LogLoadActiveInputFailedWarning(ILogger logger, string error);

    [LoggerMessage(eventId: 2, LogLevel.Warning, "An error was encountered when attmepting to get custom input shcemes, using an empty list; error: {error}")]
    private static partial void LogLoadCustomSchemesFailedWarning(ILogger logger, string error);

    [LoggerMessage(eventId: 3, LogLevel.Information, "User {userId} has changed the active scheme to {activeSchemeName} for action definition {definitionName}, for the device(s): '{deviceNames}'.")]
    private static partial void LogNewActiveSchemeInformation(ILogger logger, int userId, string deviceNames, string definitionName, string activeSchemeName);

    #endregion
}
