using Microsoft.Extensions.Logging;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Internal.Services;

internal partial class SchemeService(IInputConfigurationProvider configurationProvider, ISchemeRepository schemeRepository, IUserManager userManager, ILogger logger): ISchemeService
{
    #region Variables

    /// <summary>
    /// A cache data storage for the input schemes. It is ordered by:
    /// - Input Configuration Id
    /// - Action Definition Id
    /// - Scheme Name
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, InputScheme>>> _customSchemeLookup = [];

    private readonly Dictionary<int, PreferredInputScheme[]> _userPreferredSchemesLookup = [];

    #endregion

    #region ISchemeService

    public bool AllowCustomSchemes => schemeRepository.AllowCustomSchemes;

    public async Task<Output> LoadConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _customSchemeLookup.Clear();

        var getUserPreferredSchemes = await schemeRepository.GetPreferredSchemesAsync(cancellationToken);
        if (getUserPreferredSchemes.IsSuccessful)
        {
            // Only take one preferred scheme for each definition, even if the repository returns multiples.
            // There should only ever be 1 preferred scheme per definition, so multiples would indicate either a
            // mistake in the repository or some malicious intent
            foreach (var userPreferredSchemes in getUserPreferredSchemes.Data.Where(preferredScheme =>
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
                })
                .GroupBy(scheme => scheme.UserId))
            {
                _userPreferredSchemesLookup[userPreferredSchemes.Key] =
                    userPreferredSchemes.GroupBy(scheme => new { scheme.ConfigurationId, scheme.DefinitionName, scheme.SchemeName })
                                        .Select(schemeGroup => schemeGroup.First())
                                        .ToArray();
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
                    var configurationId = InputConfiguration.GetConfigurationId(customScheme.DeviceMaps.Select(map => map.DeviceIdentity));
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

    public ISchemeEditor? GetSchemeEditor(int userId)
    {
        var user = userManager.GetUser(userId);
        if (user is null)
        {
            return null;
        }

        return new SchemeEditor(user, configurationProvider, this);
    }

    #endregion

    #region Helpers

    internal async Task<Output> DeleteCustomSchemeAsync(string definitionName, string schemeName, CancellationToken cancellationToken = default)
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

    internal async Task<Output> SaveCustomSchemeAsync(CustomInputScheme scheme, SchemeSavePermissions savePermissions, CancellationToken cancellationToken = default)
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

        _customSchemeLookup[InputConfiguration.GetConfigurationId(scheme.DeviceMaps.Select(map => map.DeviceIdentity))][scheme.DefinitionName][scheme.Name] = scheme.ToInputScheme();
        return Out.Success();
    }


    #endregion

    #region Logging


    [LoggerMessage(eventId: 1, LogLevel.Warning, "An error was encountered when attempting to get active input schemes, using an empty list; error: {error}")]
    private static partial void LogLoadActiveInputFailedWarning(ILogger logger, string error);

    [LoggerMessage(eventId: 1, LogLevel.Warning, "An error was encountered when attmepting to get custom input shcemes, using an empty list; error: {error}")]
    private static partial void LogLoadCustomSchemesFailedWarning(ILogger logger, string error);

    #endregion
}
