using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class InputSystemConfigurationTests
{
    #region Constructor

    [Fact]
    public void Constructor_SetsExpectedValues()
    {
        // Arrange
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DeviceJoinBehavior = DevicePairingBehavior.Balanced };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        // Act
        var configuration = new InputSystemConfiguration(new[] { config }, [], joinPolicy);

        // Assert
        Assert.Single(configuration.DeviceTopologies);
        Assert.Single(configuration.InputConfigurations);
        Assert.Same(joinPolicy, configuration.JoinPolicy);
    }

    #endregion

    #region DeviceTopologies

    [Fact]
    public void DeviceTopologies_ReturnsAllTopologies()
    {
        // Arrange
        var config1 = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var config2 = new InputConfiguration(new[] { DeviceTopologyName.Mouse });

        // Act
        var configuration = new InputSystemConfiguration([config1, config2], [], new InputSystemJoinPolicy());

        // Assert
        Assert.Equal(2, configuration.DeviceTopologies.Count);
        Assert.Contains(DeviceTopologyName.Keyboard, configuration.DeviceTopologies);
        Assert.Contains(DeviceTopologyName.Mouse, configuration.DeviceTopologies);
    }

    [Fact]
    public void DeviceTopologies_NullInput_ReturnsEmpty()
    {
        // Arrange & Act
        var configuration = new InputSystemConfiguration([], [], new InputSystemJoinPolicy());

        // Assert
        Assert.Empty(configuration.DeviceTopologies);
    }

    #endregion

    #region InputConfigurations

    [Fact]
    public void InputConfigurations_ReturnsAllConfigurations()
    {
        // Arrange
        var config1 = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme1 = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config1.AddScheme(scheme1);

        var config2 = new InputConfiguration(new[] { DeviceTopologyName.Mouse });
        var scheme2 = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config2.AddScheme(scheme2);

        // Act
        var configuration = new InputSystemConfiguration([config1, config2], [], new InputSystemJoinPolicy());

        // Assert
        Assert.Equal(2, configuration.InputConfigurations.Count);
    }

    [Fact]
    public void InputConfigurations_Null_ReturnsEmptyConfigurations()
    {
        // Arrange/Act
        var configuration = new InputSystemConfiguration(null!, [], new InputSystemJoinPolicy());

        // Assert
        Assert.Empty(configuration.InputConfigurations);
    }

    #endregion

    #region Definitions

    [Fact]
    public void Definitions_ReturnsAllDefinitions()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}) };
        var def1 = new ActionDefinition("Default", actions, isDefault: true);
        var def2 = new ActionDefinition("Secondary", actions, isDefault: false);

        // Act
        var configuration = new InputSystemConfiguration([], [def1, def2], new InputSystemJoinPolicy());

        // Assert
        Assert.Equal(2, configuration.Definitions.Count);
    }

    [Fact]
    public void Definitions_NullInput_ReturnsEmpty()
    {
        // Arrange & Act
        var configuration = new InputSystemConfiguration([], null!, new InputSystemJoinPolicy());

        // Assert
        Assert.Empty(configuration.Definitions);
    }

    #endregion

    #region GetInputConfiguration

    [Fact]
    public void GetInputConfiguration_ExistingId_ReturnsConfiguration()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        // Act
        var configuration = new InputSystemConfiguration([config], [], new InputSystemJoinPolicy());
        var result = configuration.GetInputConfiguration(config.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetInputConfiguration_NonExistentId_ReturnsNull()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        // Act
        var configuration = new InputSystemConfiguration([config], [], new InputSystemJoinPolicy());
        var result = configuration.GetInputConfiguration("nonexistent");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region IsTopologySupported

    [Fact]
    public void IsTopologySupported_ExistingTopology_ReturnsTrue()
    {
        // Arrange
        var config = new InputConfiguration([DeviceTopologyName.Keyboard]);
        var configuration = new InputSystemConfiguration([config], [], new InputSystemJoinPolicy());

        // Act
        var result = configuration.IsTopologySupported(DeviceTopologyName.Keyboard);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTopologySupported_NonExistentTopology_ReturnsFalse()
    {
        // Arrange
        var configuration = new InputSystemConfiguration([], [], new InputSystemJoinPolicy());

        // Act
        var result = configuration.IsTopologySupported(DeviceTopologyName.Gamepad);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetDefinition

    [Fact]
    public void GetDefinition_ExistingName_ReturnsDefinition()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}) };
        var def = new ActionDefinition("Default", actions, isDefault: true);

        // Act
        var configuration = new InputSystemConfiguration([], [def], new InputSystemJoinPolicy());
        var result = configuration.GetDefinition("Default");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetDefinition_NonExistentName_ReturnsNull()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}) };
        var def = new ActionDefinition("Default", actions, isDefault: true);

        // Act
        var configuration = new InputSystemConfiguration([], [def], new InputSystemJoinPolicy());
        var result = configuration.GetDefinition("Other");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDefinition_IsCaseInsensitive()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}) };
        var def = new ActionDefinition("Default", actions, isDefault: true);

        // Act
        var configuration = new InputSystemConfiguration([], [def], new InputSystemJoinPolicy());
        var result = configuration.GetDefinition("default");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetDefinition_EmptyName_ReturnsNull()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}) };
        var def = new ActionDefinition("Default", actions, isDefault: true);

        // Act
        var configuration = new InputSystemConfiguration([], [def], new InputSystemJoinPolicy());
        var result = configuration.GetDefinition("");

        // Assert
        Assert.Null(result);
    }

    #endregion
}
