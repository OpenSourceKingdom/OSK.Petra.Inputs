using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class InputSystemConfigurationExtensionsTests
{
    #region GetBestFitInputConfiguration

    [Fact]
    public void GetBestFitInputConfiguration_ExactMatch_ReturnsConfiguration()
    {
        // Arrange
        var config = CreateValidConfig();
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Xbox Keyboard");

        // Act
        var result = config.GetBestFitInputConfiguration(identity);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetBestFitInputConfiguration_FamilyMatch_ReturnsConfiguration()
    {
        // Arrange
        var config = CreateValidConfig();
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Generic Keyboard");

        // Act
        var result = config.GetBestFitInputConfiguration(identity);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetBestFitInputConfiguration_WrongTopology_ReturnsNull()
    {
        // Arrange
        var config = CreateValidConfig();
        var identity = new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse");

        // Act
        var result = config.GetBestFitInputConfiguration(identity);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetBestFitInputConfiguration_GenericScheme_ReturnsAtLowerConfidence()
    {
        // Arrange
        var deviceMapsGeneric = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Generic"),
                InputMaps = [new InputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}),
                    new _Helpers.MockInput(1))]
            }
        };
        var schemeGeneric = new InputScheme("Default", "Generic", deviceMapsGeneric, isDefault: true, isCustom: false);

        var inputConfig = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        inputConfig.AddScheme(schemeGeneric);

        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DeviceJoinBehavior = DevicePairingBehavior.Balanced };

        var genericConfig = new InputSystemConfiguration([], new[] { inputConfig }, new[] { definition }, joinPolicy);

        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Steam, "Steam Keyboard");

        // Act
        var result = genericConfig.GetBestFitInputConfiguration(identity);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region Helpers

    private static InputSystemConfiguration CreateValidConfig()
    {
        var deviceMapsXbox = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Xbox Keyboard"),
                InputMaps = [new InputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}),
                    new _Helpers.MockInput(1))]
            }
        };
        var schemeXbox = new InputScheme("Default", "Default", deviceMapsXbox, isDefault: true, isCustom: false);

        var inputConfig = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        inputConfig.AddScheme(schemeXbox);

        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var definition = new ActionDefinition("Default", actions, isDefault: true);
        var joinPolicy = new InputSystemJoinPolicy { MaxUsers = 4, UserJoinBehavior = UserJoinBehavior.DeviceActivation, DeviceJoinBehavior = DevicePairingBehavior.Balanced };

        return new InputSystemConfiguration([], new[] { inputConfig }, new[] { definition }, joinPolicy);
    }

    #endregion
}
