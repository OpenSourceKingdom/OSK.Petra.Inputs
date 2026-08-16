using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests.Internal.Services;

public class InputSchemeBuilderTests
{
    #region Variables

    private readonly InputSchemeBuilder _builder;

    #endregion

    #region Constructors

    public InputSchemeBuilderTests()
    {
        _builder = new InputSchemeBuilder("TestDefinition", "TestScheme");
    }

    #endregion

    #region MakeDefault

    [Fact]
    public void MakeDefault_SetsDefault_ReturnsSelf()
    {
        // Arrange/Act
        _builder.MakeDefault();

        // Assert
        var scheme = _builder.Build();
        Assert.True(scheme.IsDefault);
    }

    [Fact]
    public void MakeDefault_MultipleCalls_DoesNotThrow()
    {
        // Act & Assert
        _builder.MakeDefault();
        _builder.MakeDefault();
    }

    #endregion

    #region WithDevice

    [Fact]
    public void WithDevice_NullMap_ThrowsArgumentNullException()
    {
        // Arrange
        DeviceInputMap? map = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithDevice(map!));
    }

    [Fact]
    public void WithDevice_DuplicateDeviceIdentity_ThrowsInvalidOperationException()
    {
        // Arrange
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test");
        var map1 = new DeviceInputMap
        {
            DeviceIdentity = identity,
            InputMaps = Array.Empty<InputActionMap>()
        };

        _builder.WithDevice(map1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithDevice(new DeviceInputMap
        {
            DeviceIdentity = identity,
            InputMaps = Array.Empty<InputActionMap>()
        }));
    }

    [Fact]
    public void WithDevice_ValidMap_AddsToLookup()
    {
        // Arrange
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test");
        var map = new DeviceInputMap
        {
            DeviceIdentity = identity,
            InputMaps = Array.Empty<InputActionMap>()
        };

        // Act
        _builder.WithDevice(map);

        // Assert
        var scheme = _builder.Build();
        Assert.Single(scheme.DeviceMaps);
    }

    [Fact]
    public void WithDevice_MultipleDevices_AddsAll()
    {
        // Arrange
        var map1 = new DeviceInputMap
        {
            DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test"),
            InputMaps = Array.Empty<InputActionMap>()
        };
        var map2 = new DeviceInputMap
        {
            DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "TestMouse"),
            InputMaps = Array.Empty<InputActionMap>()
        };

        // Act
        _builder.WithDevice(map1);
        _builder.WithDevice(map2);

        // Assert
        var scheme = _builder.Build();
        Assert.Equal(2, scheme.DeviceMaps.Count);
    }

    #endregion

    #region Build

    [Fact]
    public void Build_Defaults_ReturnsExpectedValues()
    {
        // Act
        var scheme = _builder.Build();

        // Assert
        Assert.Equal("TestScheme", scheme.Name);
        Assert.Equal("TestDefinition", scheme.DefinitionName);
        Assert.False(scheme.IsDefault);
        Assert.False(scheme.IsCustom);
    }

    [Fact]
    public void Build_MakeDefault_SetsIsDefaultTrue()
    {
        // Arrange
        _builder.MakeDefault();

        // Act
        var scheme = _builder.Build();

        // Assert
        Assert.True(scheme.IsDefault);
    }

    #endregion
}
