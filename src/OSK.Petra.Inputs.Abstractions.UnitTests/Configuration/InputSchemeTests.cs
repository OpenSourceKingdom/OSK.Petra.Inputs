using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class InputSchemeTests
{
    #region Variables

    private readonly DeviceIdentity _keyboardIdentity = new(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");
    private readonly DeviceIdentity _mouseIdentity = new(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse");

    #endregion

    #region Constructor

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_SetsDefinitionName(bool isIt)
    {
        // Arrange & Act
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: isIt, isCustom: isIt);

        // Assert
        Assert.Equal("MyScheme", scheme.Name);
        Assert.Equal("Default", scheme.DefinitionName);
        Assert.Equal(isIt, scheme.IsDefault);
        Assert.Equal(isIt, scheme.IsCustom);
    }

    #endregion

    #region DeviceMaps

    [Fact]
    public void DeviceMaps_EmptyCollection_ReturnsEmpty()
    {
        // Arrange & Act
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);

        // Assert
        Assert.Empty(scheme.DeviceMaps);
    }

    [Fact]
    public void DeviceMaps_ValidDeviceMaps_ReturnsMaps()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] },
            new DeviceInputMap { DeviceIdentity = _mouseIdentity, InputMaps = [] }
        };

        // Act
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Assert
        Assert.Equal(2, scheme.DeviceMaps.Count);
    }

    [Fact]
    public void DeviceMaps_NullDeviceMaps_ReturnsEmpty()
    {
        // Arrange & Act
        var scheme = new InputScheme("Default", "MyScheme", null!, isDefault: true, isCustom: false);

        // Assert
        Assert.Empty(scheme.DeviceMaps);
    }

    #endregion

    #region GetDeviceMap

    [Fact]
    public void GetDeviceMap_ExistingTopology_ReturnsMap()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.GetDeviceMap(_keyboardIdentity);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetDeviceMap_NonExistentTopology_ReturnsNull()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);
        var gamepadIdentity = new DeviceIdentity(DeviceTopologyName.Gamepad, DeviceFamily.Generic, "Gamepad");

        // Act
        var result = scheme.GetDeviceMap(gamepadIdentity);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetInputMap

    [Fact]
    public void GetInputMap_ValidTopologyAndInputId_ReturnsMap()
    {
        // Arrange
        var input = new MockInput(1);
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = _keyboardIdentity,
                InputMaps = [new InputActionMap(action, input)]
            }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.GetInputMap(_keyboardIdentity, 1);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetInputMap_NonExistentTopology_ReturnsNull()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.GetInputMap(_keyboardIdentity, 99);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetDeviceIdentities

    [Fact]
    public void GetDeviceIdentities_ReturnsAllDeviceIdentities()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] },
            new DeviceInputMap { DeviceIdentity = _mouseIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var identities = scheme.GetDeviceIdentities().ToList();

        // Assert
        Assert.Equal(2, identities.Count);
        Assert.Contains(_keyboardIdentity, identities);
        Assert.Contains(_mouseIdentity, identities);
    }

    [Fact]
    public void GetDeviceIdentities_EmptyMaps_ReturnsEmpty()
    {
        // Arrange
        var scheme = new InputScheme("Default", "MyScheme", [], isDefault: true, isCustom: false);

        // Act
        var identities = scheme.GetDeviceIdentities().ToList();

        // Assert
        Assert.Empty(identities);
    }

    #endregion

    #region ContainsTopology

    [Fact]
    public void ContainsTopology_ExistingTopology_ReturnsTrue()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.ContainsTopology(DeviceTopologyName.Keyboard);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsTopology_NonExistentTopology_ReturnsFalse()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.ContainsTopology(DeviceTopologyName.Gamepad);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ContainsFamily

    [Fact]
    public void ContainsFamily_ExistingFamily_ReturnsTrue()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.ContainsFamily(DeviceFamily.Xbox);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsFamily_NonExistentFamily_ReturnsFalse()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.ContainsFamily(DeviceFamily.Nintendo);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ContainsDevice

    [Fact]
    public void ContainsDevice_ExistingDevice_ReturnsTrue()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.ContainsDevice(_keyboardIdentity);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsDevice_NonExistentDevice_ReturnsFalse()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new InputScheme("Default", "MyScheme", deviceMaps, isDefault: true, isCustom: false);

        // Act
        var result = scheme.ContainsDevice(_mouseIdentity);

        // Assert
        Assert.False(result);
    }

    #endregion
}
