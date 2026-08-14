using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class CustomInputSchemeTests
{
    #region Variables

    private readonly DeviceIdentity _keyboardIdentity = new(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");

    #endregion

    #region Constructor (required properties)

    [Fact]
    public void Constructor_RequiredProperties_SetsValues()
    {
        // Arrange & Act
        var scheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "MyScheme",
            DeviceMaps = []
        };

        // Assert
        Assert.Equal("Default", scheme.DefinitionName);
        Assert.Equal("MyScheme", scheme.Name);
    }

    #endregion

    #region GetDeviceIdentities

    [Fact]
    public void GetDeviceIdentities_ReturnsAllDeviceIdentities()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };
        var scheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "MyScheme",
            DeviceMaps = deviceMaps
        };

        // Act
        var identities = scheme.GetDeviceIdentities();

        // Assert
        Assert.Single(identities);
        Assert.Contains(_keyboardIdentity, identities);
    }

    [Fact]
    public void GetDeviceIdentities_EmptyDeviceMaps_ReturnsEmpty()
    {
        // Arrange
        var scheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "MyScheme",
            DeviceMaps = []
        };

        // Act
        var identities = scheme.GetDeviceIdentities();

        // Assert
        Assert.Empty(identities);
    }

    #endregion

    #region ToInputScheme

    [Fact]
    public void ToInputScheme_ReturnsInputScheme_HasExpectedValues()
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] }
        };

        var scheme = new CustomInputScheme
        {
            DefinitionName = "Default",
            Name = "MyScheme",
            DeviceMaps = deviceMaps
        };

        // Act
        var result = scheme.ToInputScheme();

        // Assert
        Assert.Equal("Default", result.DefinitionName);
        Assert.Equal("MyScheme", result.Name);
        Assert.True(result.IsCustom);
        Assert.False(result.IsDefault);
        Assert.Single(result.DeviceMaps);
    }

    #endregion
}
