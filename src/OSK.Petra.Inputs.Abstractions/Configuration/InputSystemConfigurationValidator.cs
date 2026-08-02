using System;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public static class InputSystemConfigurationValidator
{
    #region Api

    public static InputConfigurationValidationResult ValidateConfiguration(InputSystemConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var validation = ValidateDefinitions(configuration);
        if (!validation.IsValid)
        {
            return validation;
        }

        validation = ValidateDeviceTopologies(configuration);
        if (!validation.IsValid)
        {
            return validation;
        }

        return ValidateJoinPolicy(configuration.JoinPolicy);
    }

    public static InputConfigurationValidationResult ValidateCustomScheme(InputSystemConfiguration configuration, CustomInputScheme customScheme,
        bool allowDuplicateCustomScheme)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        if (customScheme is null)
        {
            throw new ArgumentNullException(nameof(customScheme));
        }

        if (string.IsNullOrWhiteSpace(customScheme.DefinitionName))
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.Name, InputConfigurationValidation.MissingData,
                "A custom scheme must have an input definition name.");
        }

        var definition = configuration.GetDefinition(customScheme.DefinitionName);
        if (definition is null)
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.Name, InputConfigurationValidation.InvalidData,
                $"The custom scheme's definition name {customScheme.DefinitionName} does not exist and can not be used.");
        }

        if (string.IsNullOrWhiteSpace(customScheme.Name))
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.Name, InputConfigurationValidation.MissingData,
                "A custom scheme must have a scheme name.");
        }

        if (customScheme.DeviceMaps is null || !customScheme.DeviceMaps.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.DeviceMaps, InputConfigurationValidation.MissingData,
                $"The custom input scheme {customScheme.Name} on input definition {definition.Name} has no device maps.");
        }

        var scheme = customScheme.ToInputScheme();
        var inputConfiguration = configuration.GetInputConfiguration(InputConfiguration.GetConfigurationId(scheme.DeviceMaps.Select(map => map.DeviceIdentity)));
        if (inputConfiguration is null)
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.DeviceMaps, InputConfigurationValidation.InvalidData,
                $"The custom input scheme {customScheme.Name} on input definition {definition.Name} has device maps that are not supported and can not be used.");
        }

        var existingScheme = inputConfiguration.GetScheme(scheme.DefinitionName, scheme.Name);
        if (existingScheme is not null && (!existingScheme.IsCustom || (existingScheme.IsCustom && !allowDuplicateCustomScheme)))
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.Name, InputConfigurationValidation.DuplicateData,
                $"The custom scheme's name {customScheme.Name} already exists on input definition {definition.Name}.");
        }

        return ValidateInputScheme(configuration, definition, customScheme.ToInputScheme());
    }

    #endregion

    #region Helpers

    private static InputConfigurationValidationResult ValidateJoinPolicy(InputSystemJoinPolicy joinPolicy)
    {
        if (joinPolicy is null)
        {
            return InputConfigurationValidationResult.ForInputSystem(inputSystem => inputSystem.JoinPolicy, InputConfigurationValidation.MissingData,
                "Join Policy must exist.");
        }

        if (joinPolicy.MaxUsers <= 0)
        {
            return InputConfigurationValidationResult.ForJoinPolicy(policy => policy.MaxUsers, InputConfigurationValidation.InvalidData,
                "Max Users must be greater than 0.");
        }

        return InputConfigurationValidationResult.Success();
    }

    private static InputConfigurationValidationResult ValidateDefinitions(InputSystemConfiguration configuration)
    {
        if (configuration.Definitions is null || !configuration.Definitions.Any())
        {
            return InputConfigurationValidationResult.ForInputSystem(inputSystem => inputSystem.Definitions, InputConfigurationValidation.MissingData,
                "Action Definitions must exist.");
        }

        var definitions = configuration.Definitions;

        var invalidDefinitionNames = definitions.Select(definition => definition?.Name).Where(string.IsNullOrWhiteSpace);
        if (invalidDefinitionNames.Any())
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.Name, InputConfigurationValidation.MissingData,
                $"There are {invalidDefinitionNames.Count()} definitions with empty names.");
        }

        // Note: test difficulty - due to how definitions are read-only and provided at construction into a dictionary (i.e duplicate keys throw),
        // it's not entirely feasible this will occur, but validation will be done to ensure if something changes that this is still caught
        var duplicateDefinitionNames = definitions.GroupBy(definition => definition.Name, StringComparer.OrdinalIgnoreCase)
            .Where(definitionGroup => definitionGroup.Count() > 1);
        if (duplicateDefinitionNames.Any())
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.Name, InputConfigurationValidation.DuplicateData,
                $"There are {duplicateDefinitionNames.Count()} definitions with duplicate names, the names are: {string.Join(", ", duplicateDefinitionNames.Select(d => d.Key))}");
        }

        var definitionsWithoutActions = definitions.Where(definition => definition.Actions is null || !definition.Actions.Any())
                                                   .Select(definition => definition.Name);
        if (definitionsWithoutActions.Any())
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.Actions, InputConfigurationValidation.MissingData,
                $"There are {definitionsWithoutActions.Count()} definitions without actions, the names are: {string.Join(", ", definitionsWithoutActions)}.");
        }

        var defaultDefinitions = definitions.Where(definition => definition.IsDefault);
        var defaultDefinitionCount = defaultDefinitions.Count();
        if (defaultDefinitionCount is 0)
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.IsDefault, InputConfigurationValidation.InvalidData,
                "There are no definitions marked as default.");
        }

        if (defaultDefinitionCount > 1)
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.IsDefault, InputConfigurationValidation.InvalidData,
                $"There are {defaultDefinitions.Count()} definitions marked as default, but only one should be marked, the names are: {string.Join(", ", defaultDefinitions.Select(d => d.Name))}.");
        }

        foreach (var definition in definitions)
        {
            var validation = ValidateActions(definition);
            if (!validation.IsValid)
            {
                return validation;
            }
        }

        return InputConfigurationValidationResult.Success();
    }

    private static InputConfigurationValidationResult ValidateActions(ActionDefinition definition)
    {
        var actionsWithInvalidNames = definition.Actions.Where(action => string.IsNullOrWhiteSpace(action.Name));
        if (actionsWithInvalidNames.Any())
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.Actions, InputConfigurationValidation.MissingData,
                $"There are {actionsWithInvalidNames.Count()} actions with empty names on input definition {definition.Name}.");
        }

        // Note: test difficulty - due to how actions are read-only and provided at construction into a dictionary (i.e duplicate keys throw),
        // it's not entirely feasible this will occur, but validation will be done to ensure if something changes that this is still caught
        var duplicateActionNames = definition.Actions.GroupBy(action => action.Name, StringComparer.OrdinalIgnoreCase)
            .Where(actionGroup => actionGroup.Count() > 1)
            .Select(actionGroup => actionGroup.Key);
        if (duplicateActionNames.Any())
        {
            return InputConfigurationValidationResult.ForDefinition(definition => definition.Actions, InputConfigurationValidation.DuplicateData,
                $"There are {duplicateActionNames.Count()} actions with the same name on input definition {definition.Name}, the names are: {string.Join(", ", duplicateActionNames)}.");
        }

        foreach (var action in definition.Actions)
        {
            if (!action.TriggerPhases.Any())
            {
                return InputConfigurationValidationResult.ForInputAction(action => action.TriggerPhases, InputConfigurationValidation.MissingData,
                    $"There are no input trigger phases for action {action.Name} on input definition {definition.Name}.");
            }
            if (action.ActionExecutor is null)
            {
                return InputConfigurationValidationResult.ForInputAction(action => action.ActionExecutor, InputConfigurationValidation.MissingData,
                    $"There is not action executor for action {action.Name} for definition {definition.Name}.");
            }
        }

        return InputConfigurationValidationResult.Success();
    }

    private static InputConfigurationValidationResult ValidateDeviceTopologies(InputSystemConfiguration configuration)
    {
        if (configuration.InputConfigurations is null || !configuration.InputConfigurations.Any())
        {
            return InputConfigurationValidationResult.ForInputSystem(inputSystem => inputSystem.InputConfigurations, InputConfigurationValidation.MissingData,
                "Input Configurations must exist.");
        }

        // Note: test difficulty - due to how configurations are read-only and provided at construction into a dictionary (i.e duplicate keys throw),
        // it's not entirely feasible this will occur, but validation will be done to ensure if something changes that this is still caught
        var duplicateTopologies = configuration.InputConfigurations.GroupBy(configuration => configuration.Id, StringComparer.OrdinalIgnoreCase)
            .Where(configurationGroup => configurationGroup.Count() > 1);
        if (duplicateTopologies.Any())
        {
            return InputConfigurationValidationResult.ForInputConfiguration(configuration => configuration.GetDisplayName(), InputConfigurationValidation.DuplicateData,
                $"There are {duplicateTopologies.Count()} with duplicate names, the names are: {string.Join(", ", duplicateTopologies.Select(d => d.Key))}");
        }

        var configurationsWithoutSchemes = configuration.InputConfigurations.Where(config => config.Schemes is null || !config.Schemes.Any())
                                                                                     .Select(configuration => configuration.GetDisplayName());
        if (configurationsWithoutSchemes.Any())
        {
            return InputConfigurationValidationResult.ForInputConfiguration(configuration => configuration.Schemes, InputConfigurationValidation.MissingData,
                $"There are {configurationsWithoutSchemes.Count()} definitions without input schemes, the names are: {string.Join(", ", configurationsWithoutSchemes)}.");
        }

        foreach (var inputConfiguration in configuration.InputConfigurations)
        {
            var validationResult = ValidateInputConfiguration(configuration, inputConfiguration);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }
        }

        return InputConfigurationValidationResult.Success();
    }

    private static InputConfigurationValidationResult ValidateInputConfiguration(InputSystemConfiguration configuration, InputConfiguration inputConfiguration)
    {
        var schemesWithInvalidNames = inputConfiguration.Schemes.Where(scheme => string.IsNullOrWhiteSpace(scheme.Name));
        if (schemesWithInvalidNames.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.Name, InputConfigurationValidation.MissingData,
                $"There are {schemesWithInvalidNames.Count()} schemes with empty names on input configuration {inputConfiguration.GetDisplayName()}.");
        }

        var schemesWithInvalidDefinitionIds = inputConfiguration.Schemes.Where(scheme => string.IsNullOrWhiteSpace(scheme.DefinitionName));
        if (schemesWithInvalidNames.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.Name, InputConfigurationValidation.MissingData,
                $"There are {schemesWithInvalidNames.Count()} schemes with empty definition ids on input configuration {inputConfiguration.GetDisplayName()}, , the names are: {string.Join(", ", schemesWithInvalidDefinitionIds.Select(scheme => $"Definition: {scheme.DefinitionName} Scheme: {scheme.Name}"))}.");
        }

        var schemesWithUnrecognizedDefinitionIds = inputConfiguration.Schemes.Where(scheme => configuration.GetDefinition(scheme.DefinitionName) is null);
        if (schemesWithInvalidNames.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.Name, InputConfigurationValidation.InvalidData,
                $"There are {schemesWithInvalidNames.Count()} schemes with definition ids that are not registered with the input system on input configuration {inputConfiguration.GetDisplayName()}, , the names are: {string.Join(", ", schemesWithUnrecognizedDefinitionIds.Select(scheme => $"Definition: {scheme.DefinitionName} Scheme: {scheme.Name}"))}.");
        }

        // Note: test difficulty - due to how schemes are read-only and provided at construction into a dictionary (i.e duplicate keys throw),
        // it's not entirely feasible this will occur, but validation will be done to ensure if something changes that this is still caught
        var duplicateSchemeNames = inputConfiguration.Schemes.GroupBy(scheme => new { scheme.DefinitionName, scheme.Name })
           .Where(schemeGroup => schemeGroup.Count() > 1)
           .Select(schemeGroup => schemeGroup.Key);
        if (duplicateSchemeNames.Any())
        {
            return InputConfigurationValidationResult.ForInputConfiguration(inputConfig => inputConfig.Schemes, InputConfigurationValidation.DuplicateData,
                $"There are {duplicateSchemeNames.Count()} schemes with the same name on input configuration {inputConfiguration.GetDisplayName()}, the names are: {string.Join(", ", duplicateSchemeNames.Select(scheme => $"Definition: {scheme.DefinitionName} Scheme: {scheme.Name}"))}.");
        }

        var schemesMissingDeviceMaps = inputConfiguration.Schemes.Where(scheme => scheme.DeviceMaps is null || !scheme.DeviceMaps.Any());
        if (schemesMissingDeviceMaps.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.DeviceMaps, InputConfigurationValidation.MissingData,
                $"There are {schemesMissingDeviceMaps.Count()} schemes on input configuration {inputConfiguration.GetDisplayName()} that have no device maps, the names are: {string.Join(", ", schemesMissingDeviceMaps.Select(scheme => scheme.Name))}.");
        }

        var totalSchemeDefaults = inputConfiguration.Schemes.Count(scheme => scheme.IsDefault);
        if (totalSchemeDefaults is 0)
        {
            return InputConfigurationValidationResult.ForInputConfiguration(inputConfig => inputConfig.Schemes, InputConfigurationValidation.InvalidData,
                $"There are no schemes marked as default on input configuration {inputConfiguration.GetDisplayName()}.");
        }
        if (totalSchemeDefaults > 1)
        {
            return InputConfigurationValidationResult.ForInputConfiguration(inputConfig => inputConfig.Schemes, InputConfigurationValidation.InvalidData,
                $"There are {totalSchemeDefaults} schemes marked as default on input configuration {inputConfiguration.GetDisplayName()}, but only one should be marked.");
        }

        foreach (var scheme in inputConfiguration.Schemes)
        {
            var definition = configuration.GetDefinition(scheme.DefinitionName);
            var validation = ValidateInputScheme(configuration, definition!, scheme);
            if (!validation.IsValid)
            {
                return validation;
            }
        }

        return InputConfigurationValidationResult.Success();
    }

    private static InputConfigurationValidationResult ValidateInputScheme(InputSystemConfiguration configuration, ActionDefinition definition,
        InputScheme scheme)
    {
        // Note: test difficulty - due to how schemes are read-only and provided at construction into a dictionary (i.e duplicate keys throw),
        // it's not entirely feasible this will occur, but validation will be done to ensure if something changes that this is still caught
        var duplicateDeviceMaps = scheme.DeviceMaps.GroupBy(deviceMap => deviceMap.DeviceIdentity)
            .Where(deviceFamilyGroup => deviceFamilyGroup.Count() > 1)
            .Select(deviceFamilyGroup => deviceFamilyGroup.Key);
        if (duplicateDeviceMaps.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.DeviceMaps, InputConfigurationValidation.DuplicateData,
                $"There are {duplicateDeviceMaps.Count()} device maps on input scheme {scheme.Name} with input definition {definition.Name} with the same device identity, the device maps are: {string.Join(", ", duplicateDeviceMaps)}.");
        }

        var deviceMapsMissingInputMaps = scheme.DeviceMaps.Where(deviceMap => deviceMap.InputMaps is null || !deviceMap.InputMaps.Any())
            .Select(deviceMap => deviceMap.DeviceIdentity);
        if (deviceMapsMissingInputMaps.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.DeviceMaps, InputConfigurationValidation.MissingData,
                $"There are {deviceMapsMissingInputMaps.Count()} device maps that are missing input maps for scheme {scheme.Name} on input definition {definition.Name}, the device maps are: {string.Join(", ", deviceMapsMissingInputMaps)}.");
        }

        foreach (var deviceMap in scheme.DeviceMaps)
        {
            var deviceMapValidation = ValidateDeviceMap(configuration, definition, scheme, deviceMap);
            if (!deviceMapValidation.IsValid)
            {
                return deviceMapValidation;
            }
        }

        var duplicateActions = scheme.DeviceMaps.SelectMany(map => map.InputMaps)
            .GroupBy(map => map.ActionName)
            .Where(actionMapGroup => actionMapGroup.Count() > 1)
            .Select(actionMapGroup => actionMapGroup.Key);
        if (duplicateActions.Any())
        {
            return InputConfigurationValidationResult.ForScheme(scheme => scheme.DeviceMaps, InputConfigurationValidation.DuplicateData,
                $"There are {duplicateActions.Count()} input maps that share the same action name across the device maps for scheme {scheme.Name} on input definition {definition.Name}, the action names are: {string.Join(", ", duplicateActions)}");
        }

        return InputConfigurationValidationResult.Success();
    }

    private static InputConfigurationValidationResult ValidateDeviceMap(InputSystemConfiguration configuration, ActionDefinition definition, InputScheme scheme, DeviceInputMap deviceMap)
    {
        var topologyDescriptor = configuration.GetTopologyDescriptor(deviceMap.DeviceIdentity.TopologyName);
        if (topologyDescriptor is null)
        {
            return InputConfigurationValidationResult.ForDeviceMap(map => map.DeviceIdentity, InputConfigurationValidation.InvalidData,
                $"The input scheme {scheme.Name} on input defintion {definition.Name} uses a device identity that is not configured for the input system, the device is: {deviceMap.DeviceIdentity}.");
        }

        var invalidInputIds = deviceMap.InputMaps.Where(map => !topologyDescriptor.IsCompatibleInput(map.Input))
            .Select(map => map.Input.Id);
        if (invalidInputIds.Any())
        {
            return InputConfigurationValidationResult.ForDeviceMap(map => map.InputMaps, InputConfigurationValidation.InvalidData,
                $"There are {invalidInputIds.Count()} input maps that use input ids that don't exist for the device map {deviceMap.DeviceIdentity} with scheme {scheme.Name} on input definition {definition.Name}, the invalid ids are: {string.Join(",", invalidInputIds.Distinct())}.");
        }

        var duplicateInputIds = deviceMap.InputMaps.GroupBy(map => map.Input.Id)
            .Where(mapGroup => mapGroup.Count() > 1)
            .Select(mapGroup => mapGroup.Key);
        if (duplicateInputIds.Any()) 
        {
            return InputConfigurationValidationResult.ForDeviceMap(map => map.InputMaps, InputConfigurationValidation.DuplicateData,
                $"There are {duplicateInputIds.Count()} input ids for the device map {deviceMap.DeviceIdentity} with scheme {scheme.Name} on input definition {definition.Name}, the duplicate ids are: {string.Join(", ", duplicateInputIds)}.");
        }

        var inputsMissingActionNames = deviceMap.InputMaps.Where(map => string.IsNullOrWhiteSpace(map.ActionName))
            .Select(map => map.Input.Id);
        if (inputsMissingActionNames.Any()) 
        {
            return InputConfigurationValidationResult.ForDeviceMap(map => map.InputMaps, InputConfigurationValidation.MissingData,
                $"There are {inputsMissingActionNames.Count()} input maps missing action names for device map {deviceMap.DeviceIdentity} with scheme {scheme.Name} on input definition {definition.Name}, the input map ids are: {string.Join(", ", inputsMissingActionNames)}.");
        }

        var invalidActionNames = deviceMap.InputMaps.Where(map => definition.GetAction(map.ActionName) is null);
        if (invalidActionNames.Any()) 
        {
            return InputConfigurationValidationResult.ForDeviceMap(map => map.InputMaps, InputConfigurationValidation.InvalidData,
                $"There are {invalidActionNames.Count()} input maps with action names that don't exist for device map {deviceMap.DeviceIdentity} with scheme {scheme.Name} on input definition {definition.Name}, the invalid action names are: {string.Join(", ", invalidActionNames.Select(map => map.ActionName).Distinct())}.");
        }

        var duplicateActionNames = deviceMap.InputMaps.GroupBy(map => map.ActionName)
            .Where(mapGroup => mapGroup.Count() > 1)
            .Select(mapGroup => mapGroup.Key);
        if (duplicateActionNames.Any())
        {
            return InputConfigurationValidationResult.ForDeviceMap(map => map.InputMaps, InputConfigurationValidation.DuplicateData,
                $"There are {duplicateActionNames.Count()} duplicate action names for device map {deviceMap.DeviceIdentity} with scheme {scheme.Name} on input defintion {definition.Name}, the action names are: {string.Join(", ", duplicateActionNames)}.");
        }

        return InputConfigurationValidationResult.Success();
    }

    #endregion
}
