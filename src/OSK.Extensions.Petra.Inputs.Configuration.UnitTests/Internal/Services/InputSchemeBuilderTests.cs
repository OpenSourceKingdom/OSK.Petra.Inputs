using Moq;
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
        _builder = new InputSchemeBuilder("TestScheme");
    }

    #endregion

    #region MakeDefault

    [Fact]
    public void MakeDefault_SetsDefault_ReturnsSelf()
    {
        // Arrange/Act
        _builder.MakeDefault();

        // Assert
        var scheme = _builder.Build(new ActionDefinition("Abc", [], false));
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

    #region WithMap

    [Fact]
    public void WithMap_NullInput_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithMap(new DeviceIdentity(), null!, "Abc"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void WithMap_EmptyActionName_ThrowsArgumentNullException(string? name)
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithMap(new DeviceIdentity(), Mock.Of<IInput>(), name!));
    }

    [Fact]
    public void WithMap_ValidMap_AddsToLookup()
    {
        // Arrange
        var identity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test");

        // Act
        _builder.WithMap(identity, Mock.Of<IInput>(), "Abc");

        // Assert
        var scheme = _builder.Build(new ActionDefinition("Abc", [new InputAction("Abc", new HashSet<InputPhase>(), _ => { })], false));
        Assert.Single(scheme.DeviceMaps);
    }

    [Fact]
    public void WithMap_MultipleDevices_AddsAll()
    {
        // Arrange/Act
        _builder.WithMap(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test"), Mock.Of<IInput>(), "Abc");
        _builder.WithMap(new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Test"), Mock.Of<IInput>(), "Def");

        // Assert
        var scheme = _builder.Build(new ActionDefinition("Abc", [
            new InputAction("Abc", new HashSet<InputPhase>(), _ => { }),
            new InputAction("Def", new HashSet<InputPhase>(), _ => { })
        ], false));
        Assert.Equal(2, scheme.DeviceMaps.Count);
    }

    #endregion

    #region Build

    [Fact]
    public void Build_Defaults_ReturnsExpectedValues()
    {
        // Act
        var scheme = _builder.Build(new ActionDefinition("TestDefinition", [], false));

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
        var scheme = _builder.Build(new ActionDefinition("Abc", [], false));

        // Assert
        Assert.True(scheme.IsDefault);
    }

    #endregion
}
