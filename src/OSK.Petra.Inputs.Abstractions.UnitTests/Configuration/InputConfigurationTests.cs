using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class InputConfigurationTests
{
    #region Constructor

    [Fact]
    public void Constructor_NullTopologyNames_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputConfiguration(null!));
    }

    [Fact]
    public void Constructor_ValidTopologyNames_SetsPropertiesCorrectly()
    {
        // Arrange
        var topologies = new[] { DeviceTopologyName.Mouse, DeviceTopologyName.Keyboard };

        // Act
        var config = new InputConfiguration(topologies);

        // Assert
        Assert.Equal("Keyboard.Mouse", config.Id);
        Assert.Equal(2, config.TopologyNames.Count);
        Assert.Contains(DeviceTopologyName.Keyboard, config.TopologyNames);
        Assert.Contains(DeviceTopologyName.Mouse, config.TopologyNames);
    }

    #endregion

    #region GetConfigurationId_EnumerableDeviceIdentity

    [Fact]
    public void GetConfigurationId_SingleDeviceIdentity_ReturnsTopologyName()
    {
        // Arrange
        var identities = new[] { new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard") };

        // Act
        var result = InputConfiguration.GetConfigurationId(identities);

        // Assert
        Assert.Equal("Keyboard", result);
    }

    [Fact]
    public void GetConfigurationId_MultipleDeviceIdentities_ReturnsAlphabeticallyOrdered()
    {
        // Arrange
        var identities = new[]
        {
            new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse"),
            new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard")
        };

        // Act
        var result = InputConfiguration.GetConfigurationId(identities);

        // Assert
        Assert.Equal("Keyboard.Mouse", result);
    }

    [Fact]
    public void GetConfigurationId_SameDevicesDifferentOrder_ReturnsSameId()
    {
        // Arrange
        var identitiesA = new[]
        {
            new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard"),
            new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse")
        };
        var identitiesB = new[]
        {
            new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse"),
            new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard")
        };

        // Act
        var resultA = InputConfiguration.GetConfigurationId(identitiesA);
        var resultB = InputConfiguration.GetConfigurationId(identitiesB);

        // Assert
        Assert.Equal(resultA, resultB);
    }

    [Fact]
    public void GetConfigurationId_DuplicateDevices_RemovesDuplicates()
    {
        // Arrange
        var identities = new[]
        {
            new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard"),
            new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "OtherKeyboard")
        };

        // Act
        var result = InputConfiguration.GetConfigurationId(identities);

        // Assert
        Assert.Equal("Keyboard", result);
    }

    #endregion

    #region GetConfigurationId_EnumerableTopologyName

    [Fact]
    public void GetConfigurationId_SingleTopology_ReturnsTopologyName()
    {
        // Arrange
        var topologies = new[] { DeviceTopologyName.Gamepad };

        // Act
        var result = InputConfiguration.GetConfigurationId(topologies);

        // Assert
        Assert.Equal("Gamepad", result);
    }

    [Fact]
    public void GetConfigurationId_MultipleTopologies_ReturnsAlphabeticallyOrdered()
    {
        // Arrange
        var topologies = new[] { DeviceTopologyName.Gamepad, DeviceTopologyName.Keyboard };

        // Act
        var result = InputConfiguration.GetConfigurationId(topologies);

        // Assert
        Assert.Equal("Gamepad.Keyboard", result);
    }

    [Fact]
    public void GetConfigurationId_SameTopologiesDifferentOrder_ReturnsSameId()
    {
        // Arrange
        var topologiesA = new[]
        {
            DeviceTopologyName.Keyboard,
            DeviceTopologyName.Mouse
        };
        var topologiesB = new[]
        {
            DeviceTopologyName.Mouse,
            DeviceTopologyName.Keyboard            
        };

        // Act
        var resultA = InputConfiguration.GetConfigurationId(topologiesA);
        var resultB = InputConfiguration.GetConfigurationId(topologiesB);

        // Assert
        Assert.Equal(resultA, resultB);
    }

    [Fact]
    public void GetConfigurationId_DuplicateTopologies_RemovesDuplicates()
    {
        // Arrange
        var topologies = new[]
        {
            DeviceTopologyName.Keyboard,
            DeviceTopologyName.Keyboard
        };

        // Act
        var result = InputConfiguration.GetConfigurationId(topologies);

        // Assert
        Assert.Equal("Keyboard", result);
    }

    #endregion

    #region GetScheme

    [Fact]
    public void GetScheme_ExistingScheme_ReturnsScheme()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        // Act
        var result = config.GetScheme("Default", "MyScheme");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("MyScheme", result!.Name);
    }

    [Fact]
    public void GetScheme_NonExistentDefinition_ReturnsNull()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        // Act
        var result = config.GetScheme("OtherDef", "MyScheme");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetScheme_NonExistentSchemeName_ReturnsNull()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        // Act
        var result = config.GetScheme("Default", "OtherScheme");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetScheme_IsCaseInsensitive()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);

        // Act
        var result = config.GetScheme("default", "myscheme");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("MyScheme", result!.Name);
    }

    #endregion

    #region GetDeviceSupportConfidence

    [Fact]
    public void GetDeviceSupportConfidence_NoSchemes_ReturnsZero()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");

        // Act
        var result = config.GetDeviceSupportConfidence(identity);

        // Assert
        Assert.Equal(0f, result);
    }

    [Fact]
    public void GetDeviceSupportConfidence_TopologyNotInConfiguration_ReturnsZero()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        config.AddScheme(scheme);
        var identity = new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse");

        // Act
        var result = config.GetDeviceSupportConfidence(identity);

        // Assert
        Assert.Equal(0f, result);
    }

    [Fact]
    public void GetDeviceSupportConfidence_DeviceInScheme_ReturnsOne()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard"),
                InputMaps = [new DeviceInputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}),
                    1)]
            }
        };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        config.AddScheme(scheme);
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");

        // Act
        var result = config.GetDeviceSupportConfidence(identity);

        // Assert
        Assert.Equal(1f, result);
    }

    [Fact]
    public void GetDeviceSupportConfidence_DeviceFamilyInScheme_ReturnsPointSevenFive()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.PlayStation, "Xbox Keyboard"),
                InputMaps = [new DeviceInputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}),
                    1)]
            }
        };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        config.AddScheme(scheme);
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.PlayStation, "PlayStation Keyboard");

        // Act
        var result = config.GetDeviceSupportConfidence(identity);

        // Assert
        Assert.Equal(0.75f, result);
    }

    [Fact]
    public void GetDeviceSupportConfidence_GenericScheme_ReturnsPointFive()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Generic Keyboard"),
                InputMaps = [new DeviceInputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}),
                    1)]
            }
        };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        config.AddScheme(scheme);
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Steam, "Steam Deck Keyboard");

        // Act
        var result = config.GetDeviceSupportConfidence(identity);

        // Assert
        Assert.Equal(0.5f, result);
    }

    [Fact]
    public void GetDeviceSupportConfidence_NoGenericScheme_ReturnsPointOne()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Xbox Keyboard"),
                InputMaps = [new DeviceInputActionMap(
                    new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}),
                    1)]
            }
        };
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        config.AddScheme(scheme);
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Nintendo, "Nintendo Keyboard");

        // Act
        var result = config.GetDeviceSupportConfidence(identity);

        // Assert
        Assert.Equal(0.1f, result);
    }

    #endregion

    #region Contains

    [Fact]
    public void Contains_DeviceInTopology_ReturnsTrue()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");

        // Act
        var result = config.Contains(identity);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_DeviceNotInTopology_ReturnsFalse()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var identity = new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse");

        // Act
        var result = config.Contains(identity);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetDisplayName

    [Fact]
    public void GetDisplayName_EmptyTopologies_ReturnsEmptyString()
    {
        // Arrange
        var config = new InputConfiguration([]);

        // Act
        var result = config.GetDisplayName();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetDisplayName_SingleTopology_ReturnsTopologyName()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });

        // Act
        var result = config.GetDisplayName();

        // Assert
        Assert.Equal("Keyboard", result);
    }

    [Fact]
    public void GetDisplayName_TwoTopologies_ReturnsAndSeparated()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard, DeviceTopologyName.Mouse });

        // Act
        var result = config.GetDisplayName();

        // Assert
        Assert.Equal("Keyboard and Mouse", result);
    }

    [Fact]
    public void GetDisplayName_ThreeTopologies_ReturnsCommaAndSeparated()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard, DeviceTopologyName.Mouse, DeviceTopologyName.Gamepad });

        // Act
        var result = config.GetDisplayName();

        // Assert
        Assert.Equal("Keyboard, Mouse, and Gamepad", result);
    }

    #endregion

    #region AddScheme

    [Fact]
    public void AddScheme_NullScheme_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => config.AddScheme(null!));
    }

    [Fact]
    public void AddScheme_EmptySchemeName_ThrowsInvalidOperationException()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "", [], isDefault: true, isCustom: false);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => config.AddScheme(scheme));
    }

    [Fact]
    public void AddScheme_EmptyDefinitionName_ThrowsInvalidOperationException()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("", "MyScheme", [], isDefault: true, isCustom: false);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => config.AddScheme(scheme));
    }

    [Fact]
    public void AddScheme_DuplicateSchemeName_ThrowsInvalidOperationException()
    {
        // Arrange
        var config = new InputConfiguration([DeviceTopologyName.Keyboard]);
        var scheme1 = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);
        var scheme2 = new InputScheme("Default", "MyScheme", [], isDefault: false, isCustom: false);
        config.AddScheme(scheme1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => config.AddScheme(scheme2));
    }

    [Fact]
    public void AddScheme_ValidScheme_AddsToSchemes()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);

        // Act
        config.AddScheme(scheme);

        // Assert
        Assert.Single(config.Schemes);
        Assert.Contains(scheme, config.Schemes);
    }

    [Fact]
    public void AddScheme_ValidScheme_BecomesGettable()
    {
        // Arrange
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);

        // Act
        config.AddScheme(scheme);

        // Assert
        var found = config.GetScheme("Default", "MyScheme");
        Assert.NotNull(found);
    }

    #endregion
}
