using Moq;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class InputSystemConfigurationValidatorTests
{
    #region Variables

    private static readonly DeviceIdentity _keyboardIdentity = new(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");
    private static readonly DeviceIdentity _mouseIdentity = new(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse");

    #endregion

    #region ValidateConfiguration

    [Fact]
    public void ValidateConfiguration_NullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        InputSystemConfiguration configuration = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => InputSystemConfigurationValidator.ValidateConfiguration(configuration));
    }

    [Fact]
    public void ValidateConfiguration_ValidConfiguration_ReturnsSuccess()
    {
        // Arrange
        var configuration = CreateValidConfiguration();

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(InputConfigurationValidation.Ok, result.Result);
    }

    [Fact]
    public void ValidateConfiguration_NoDefinitions_ReturnsMissingData()
    {
        // Arrange
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var configuration = new InputSystemConfiguration(new[] { config }, [], joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.InputSystem, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_EmptyDefinitionName_ReturnsMissingData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var configuration = CreateValidConfiguration();
        // Replace definitions via reflection isn't available, so we create fresh
        var config2 = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme2 = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config2.AddScheme(scheme2);
        configuration = new InputSystemConfiguration(new[] { config2 }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.Definition, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_DefinitionWithoutActions_ReturnsMissingData()
    {
        // Arrange
        var definition = new ActionDefinition("Default", [], isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.Definition, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_NoDefaultDefinition_ReturnsInvalidData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: false);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.Definition, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_MultipleDefaultDefinitions_ReturnsInvalidData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition1 = new ActionDefinition("Default1", actions, isDefault: true);
        var definition2 = new ActionDefinition("Default2", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition1, definition2 }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.Definition, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_ActionWithEmptyName_ReturnsMissingData()
    {
        // Arrange
        var actions = new[]
        {
            new InputAction("", new HashSet<InputPhase> { InputPhase.Start }, ctx => { })
        };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.Definition, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_ActionWithNoTriggerPhases_ReturnsMissingData()
    {
        // Arrange
        var actions = new[]
        {
            new InputAction("Move", new HashSet<InputPhase>(), ctx => { })
        };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.InputAction, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_ActionWithNullExecutor_ReturnsMissingData()
    {
        // Arrange
        var actions = new[]
        {
            new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, null!)
        };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.InputAction, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_NoInputConfigurations_ReturnsMissingData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var configuration = new InputSystemConfiguration([], new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.InputSystem, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_ConfigurationWithoutSchemes_ReturnsMissingData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.InputConfiguration, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_SchemeWithUnrecognizedDefinition_ReturnsInvalidData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("OtherDef", "MyScheme", 
            [
                new() { DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Abc"), InputMaps = [new(new("Default", new HashSet<InputPhase>(), _ => { }), 0)] }
            ],
            isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_SchemeWithoutDeviceMaps_ReturnsMissingData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_NoDefaultScheme_ReturnsInvalidData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [new() { DeviceIdentity = new DeviceIdentity(), InputMaps = [] }], isDefault: false, isCustom: false);
        config.AddScheme(scheme);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.InputConfiguration, result.ConfigurationType);
    }

    [Fact]
    public void ValidateConfiguration_MultipleDefaultSchemes_ReturnsInvalidData()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme1 = new InputScheme("Default", "Scheme1", [new() { DeviceIdentity = new DeviceIdentity(), InputMaps = [] }], isDefault: true, isCustom: false);
        var scheme2 = new InputScheme("Default", "Scheme2", [new() { DeviceIdentity = new DeviceIdentity(), InputMaps = [] }], isDefault: true, isCustom: false);
        config.AddScheme(scheme1);
        config.AddScheme(scheme2);

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        // Act
        var result = InputSystemConfigurationValidator.ValidateConfiguration(configuration);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.InputConfiguration, result.ConfigurationType);
    }

    #endregion

    #region ValidateJoinPolicy

    [Fact]
    public void ValidateJoinPolicy_JoinPolicyWithZeroMaxUsers_ReturnsInvalidData()
    {
        // Arrange
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 0, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };

        // Act
        var result = InputSystemConfigurationValidator.ValidateJoinPolicy(joinPolicy);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.JoinPolicy, result.ConfigurationType);
    }

    [Fact]
    public void ValidateJoinPolicy_JoinPolicyWithNegativeMaxUsers_ReturnsInvalidData()
    {
        // Arrange
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = -1, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };

        // Act
        var result = InputSystemConfigurationValidator.ValidateJoinPolicy(joinPolicy);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.JoinPolicy, result.ConfigurationType);
    }

    #endregion

    #region ValidateCustomScheme

    [Fact]
    public void ValidateCustomScheme_NullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        InputSystemConfiguration configuration = null!;
        var customScheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "Custom",
            DeviceMaps = []
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, false));
    }

    [Fact]
    public void ValidateCustomScheme_NullCustomScheme_ThrowsArgumentNullException()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        CustomInputScheme customScheme = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, false));
    }

    [Fact]
    public void ValidateCustomScheme_EmptyDefinitionName_ReturnsMissingData()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var customScheme = new CustomInputScheme
        {
            DefinitionName = "",
            Name = "Custom",
            DeviceMaps = []
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
    }

    [Fact]
    public void ValidateCustomScheme_InvalidDefinitionName_ReturnsInvalidData()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var customScheme = new CustomInputScheme
        {
            DefinitionName = "NonExistent",
            Name = "Custom",
            DeviceMaps = []
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.Definition, result.ConfigurationType);
    }

    [Fact]
    public void ValidateCustomScheme_EmptySchemeName_ReturnsMissingData()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var customScheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "",
            DeviceMaps = []
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
    }

    [Fact]
    public void ValidateCustomScheme_EmptyDeviceMaps_ReturnsMissingData()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var customScheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "Custom",
            DeviceMaps = []
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
    }

    [Fact]
    public void ValidateCustomScheme_UnsupportedDeviceTopology_ReturnsInvalidData()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var unsupportedIdentity = new DeviceIdentity(DeviceTopologyName.Gamepad, DeviceFamily.Generic, "Gamepad");
        var customScheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "Custom",
            DeviceMaps = [new DeviceInputMap { DeviceIdentity = unsupportedIdentity, InputMaps = [] }]
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
    }

    [Fact]
    public void ValidateCustomScheme_DuplicateCustomSchemeName_ReturnsDuplicateData()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = _keyboardIdentity,
                InputMaps = [new InputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }),
                    1)]
            }
        };
        var existingScheme = new InputScheme("Default", "Custom", deviceMaps, isDefault: false, isCustom: true);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(existingScheme);

        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        var customScheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "Custom",
            DeviceMaps = deviceMaps
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, allowDuplicateCustomScheme: false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(InputConfigurationValidation.DuplicateData, result.Result);
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
    }

    [Fact]
    public void ValidateCustomScheme_DuplicateCustomSchemeName_Allowed_ReturnsSuccess()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = _keyboardIdentity,
                InputMaps = [new InputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }),
                    1)]
            }
        };
        var existingScheme = new InputScheme("Default", "Custom", deviceMaps, isDefault: false, isCustom: true);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(existingScheme);

        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        var customScheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "Custom",
            DeviceMaps = deviceMaps
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, allowDuplicateCustomScheme: true);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateCustomScheme_DuplicateCustomSchemeNameAllowed_SchemeIsNotCustom_ReturnsInvalidData()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = _keyboardIdentity,
                InputMaps = [new InputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }),
                    1)]
            }
        };
        var existingScheme = new InputScheme("Default", "Custom", deviceMaps, isDefault: false, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(existingScheme);

        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DevicePairingBehavior = DevicePairingBehavior.Balanced };

        var configuration = new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));

        var customScheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "Custom",
            DeviceMaps = deviceMaps
        };

        // Act
        var result = InputSystemConfigurationValidator.ValidateCustomScheme(configuration, customScheme, allowDuplicateCustomScheme: true);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("IsCustom", result.TargetName);
        Assert.Equal(InputConfigurationValidation.InvalidData, result.Result);
    }


    #endregion

    #region Helpers

    private static InputSystemConfiguration CreateValidConfiguration()
    {
        var actions = new[]
        {
            new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }, "Moves the cursor"),
            new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start, InputPhase.End }, ctx => { }, "Clicks")
        };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = _keyboardIdentity,
                InputMaps = new[]
                {
                    new InputActionMap(actions[0], 1)
                }
            },
            new DeviceInputMap
            {
                DeviceIdentity = _mouseIdentity,
                InputMaps = new[]
                {
                    new InputActionMap(actions[1], 2)
                }
            }
        };

        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard, DeviceTopologyName.Mouse });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DevicePairingBehavior = DevicePairingBehavior.Balanced
        };

        return new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([]));
    }

    #endregion
}
