using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Extensions.Petra.Inputs.Configuration.UnitTests._Helpers;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests.Internal.Services;

public class InputSystemBuilderTests
{
    #region Variables

    private readonly InputSystemBuilder _builder;
    private readonly DeviceIdentity _keyboardIdentity;
    private readonly DeviceIdentity _mouseIdentity;

    #endregion

    #region Constructors

    public InputSystemBuilderTests()
    {
        _builder = new InputSystemBuilder();
        _keyboardIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "TestKeyboard");
        _mouseIdentity = new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "TestMouse");
    }

    #endregion

    #region UseSchemeRepository

    [Fact]
    public void UseSchemeRepository_SetsType()
    {
        // Arrange/Act
        _builder.UseSchemeRepository<TestSchemeRepository>();

        // Assert
        Assert.Equal(typeof(TestSchemeRepository), _builder.ScheemRepositoryType);
    }

    #endregion

    #region WithActionDefinition

    [Fact]
    public void WithActionDefinition_NullDefinition_ThrowsArgumentNullException()
    {
        // Arrange
        ActionDefinition? definition = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithActionDefinition(definition!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void WithActionDefinition_EmptyName_ThrowsInvalidOperationException(string? name)
    {
        // Arrange
        var action = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var definition = new ActionDefinition(name!, new[] { action }, isDefault: false);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithActionDefinition(definition));
    }

    [Fact]
    public void WithActionDefinition_DuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var action = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var definition1 = new ActionDefinition("Test", new[] { action }, isDefault: false);
        var definition2 = new ActionDefinition("Test", new[] { action }, isDefault: true);

        _builder.WithActionDefinition(definition1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithActionDefinition(definition2));
    }


    [Fact]
    public void WithActionDefinition_CaseInsensitive_DuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var action = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var definition1 = new ActionDefinition("Test", new[] { action }, isDefault: false);
        var definition2 = new ActionDefinition("tEsT", new[] { action }, isDefault: true);

        _builder.WithActionDefinition(definition1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithActionDefinition(definition2));
    }


    [Fact]
    public void WithActionDefinition_ValidDefinition_AddsToLookup()
    {
        // Arrange
        var action = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var definition = new ActionDefinition("Test", new[] { action }, isDefault: false);

        // Act
        _builder.WithActionDefinition(definition);

        // Assert
        var config = _builder.BuildConfiguration();
        Assert.Single(config.Definitions);
        Assert.Equal("Test", config.Definitions.First().Name);
    }

    #endregion

    #region WithInputScheme

    [Fact]
    public void WithInputScheme_NullScheme_ThrowsArgumentNullException()
    {
        // Arrange
        InputScheme? scheme = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithInputScheme(scheme!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void WithInputScheme_EmptyName_ThrowsInvalidOperationException(string? name)
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>();
        var scheme = new InputScheme("TestDef", name!, deviceMaps, false, false);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithInputScheme(scheme));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void WithInputScheme_EmptyDefinitionName_ThrowsInvalidOperationException(string? name)
    {
        // Arrange
        var deviceMaps = new List<DeviceInputMap>();
        var scheme = new InputScheme(name!, "TestScheme", deviceMaps, false, false);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithInputScheme(scheme));
    }

    [Fact]
    public void WithInputScheme_DuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var map1 = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = Array.Empty<InputActionMap>()
        };
        var scheme1 = new InputScheme("TestDef", "MyScheme", new[] { map1 }, false, false);

        var map2 = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = Array.Empty<InputActionMap>()
        };
        var scheme2 = new InputScheme("TestDef", "MyScheme", new[] { map2 }, false, false);

        _builder.WithInputScheme(scheme1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithInputScheme(scheme2));
    }

    [Fact]
    public void WithInputScheme_CaseInsensitive_DuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var map1 = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = Array.Empty<InputActionMap>()
        };
        var scheme1 = new InputScheme("TestDef", "MyScheme", new[] { map1 }, false, false);

        var map2 = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = Array.Empty<InputActionMap>()
        };
        var scheme2 = new InputScheme("TestDef", "MyScHeMe", new[] { map2 }, false, false);

        _builder.WithInputScheme(scheme1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithInputScheme(scheme2));
    }

    [Fact]
    public void WithInputScheme_ValidScheme_AddsToLookup()
    {
        // Arrange
        var map = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = Array.Empty<InputActionMap>()
        };
        var scheme = new InputScheme("TestDef", "MyScheme", new[] { map }, false, false);

        // Act
        _builder.WithInputScheme(scheme);

        // Assert
        var config = _builder.BuildConfiguration();
        Assert.NotEmpty(config.InputConfigurations);
    }

    #endregion

    #region WithJoinPolicy_NullConfigurator

    [Fact]
    public void WithJoinPolicy_NullConfigurator_ThrowsArgumentNullException()
    {
        // Arrange
        Action<InputSystemJoinPolicy>? configurator = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithJoinPolicy(configurator!));
    }

    [Fact]
    public void WithJoinPolicy_SetsConfigurator()
    {
        // Arrange
        InputSystemJoinPolicy? capturedPolicy = null;

        // Act
        _builder.WithJoinPolicy(policy =>
        {
            capturedPolicy = policy;
        });

        var config = _builder.BuildConfiguration();

        // Assert
        Assert.NotNull(capturedPolicy);
    }

    #endregion

    #region BuildConfiguration

    [Fact]
    public void BuildConfiguration_ReturnsValidInputSystemConfiguration()
    {
        // Arrange
        var map = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = Array.Empty<InputActionMap>()
        };
        var scheme = new InputScheme("TestDef", "MyScheme", new[] { map }, false, false);

        _builder.WithInputScheme(scheme);

        // Act
        var config = _builder.BuildConfiguration();

        // Assert
        Assert.NotNull(config);
        Assert.Single(config.DeviceTopologies);
        Assert.Contains(_keyboardIdentity.TopologyName, config.DeviceTopologies);
    }

    [Fact]
    public void BuildConfiguration_DefaultsJoinPolicy()
    {
        // Act
        var config = _builder.BuildConfiguration();

        // Assert
        Assert.Equal(1, config.JoinPolicy.MaxUsers);
        Assert.Equal(DevicePairingBehavior.Balanced, config.JoinPolicy.DeviceJoinBehavior);
        Assert.Equal(UserJoinBehavior.DeviceActivation, config.JoinPolicy.UserJoinBehavior);
    }

    [Fact]
    public void BuildConfiguration_WithDefinition_DefinitionsIncluded()
    {
        // Arrange
        var action = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var definition = new ActionDefinition("Test", new[] { action }, isDefault: false);

        _builder.WithActionDefinition(definition);

        // Act
        var config = _builder.BuildConfiguration();

        // Assert
        Assert.Single(config.Definitions);
    }

    [Fact]
    public void BuildConfiguration_WithScheme_ConfigurationsIncluded()
    {
        // Arrange
        var map = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = Array.Empty<InputActionMap>()
        };
        var scheme = new InputScheme("TestDef", "MyScheme", new[] { map }, false, false);

        _builder.WithInputScheme(scheme);

        // Act
        var config = _builder.BuildConfiguration();

        // Assert
        Assert.NotEmpty(config.InputConfigurations);
    }

    #endregion
}
