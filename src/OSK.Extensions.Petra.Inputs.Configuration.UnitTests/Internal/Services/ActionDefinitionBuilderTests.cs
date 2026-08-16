using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests.Internal.Services;

public class ActionDefinitionBuilderTests
{
    #region Variables

    private readonly ActionDefinitionBuilder _builder;

    #endregion

    #region Constructors

    public ActionDefinitionBuilderTests()
    {
        _builder = new ActionDefinitionBuilder("TestDefinition");
    }

    #endregion

    #region MakeDefault

    [Fact]
    public void MakeDefault_SetsIsDefault_ReturnsSelf()
    {
        // Arrange/Act
        _builder.MakeDefault();

        // Assert
        var definition = _builder.Build();
        Assert.True(definition.IsDefault);
    }

    [Fact]
    public void MakeDefault_MultipleCalls_DoesNotThrow()
    {
        // Act & Assert
        _builder.MakeDefault();
        _builder.MakeDefault();
    }

    #endregion

    #region WithAction

    [Fact]
    public void WithAction_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        InputAction? action = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithAction(action!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void WithAction_EmptyName_ThrowsArgumentNullException(string? name)
    {
        // Arrange
        var action = new InputAction(name!, new HashSet<InputPhase> { InputPhase.Start }, ctx => { });

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithAction(action));
    }

    [Fact]
    public void WithAction_NullExecutor_ThrowsArgumentNullException()
    {
        // Arrange
        var action = new InputAction("ValidAction", new HashSet<InputPhase> { InputPhase.Start }, null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithAction(action));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithAction_EmptyTriggerPhases_ThrowsArgumentNullException(bool useEmpty)
    {
        // Arrange
        var action = new InputAction("ValidAction", useEmpty ? new HashSet<InputPhase>() : null!, ctx => { });

        // Act & Assert
        if (useEmpty)
        {
            Assert.Throws<InvalidOperationException>(() => _builder.WithAction(action));
        }
        else
        {
            Assert.Throws<ArgumentNullException>(() => _builder.WithAction(action));
        }
    }

    [Fact]
    public void WithAction_DuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var action1 = new InputAction("Duplicate", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var action2 = new InputAction("Duplicate", new HashSet<InputPhase> { InputPhase.End }, ctx => { });

        _builder.WithAction(action1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithAction(action2));
    }

    [Fact]
    public void WithAction_CaseInsensitiveName_Duplicate_ThrowsInvalidOperationException()
    {
        // Arrange
        var action1 = new InputAction("Test", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var action2 = new InputAction("TEST", new HashSet<InputPhase> { InputPhase.End }, ctx => { });

        _builder.WithAction(action1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithAction(action2));
    }

    [Fact]
    public void WithAction_ValidAction_AddsToLookup()
    {
        // Arrange
        var action = new InputAction("TestAction", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });

        // Act
        _builder.WithAction(action);

        // Assert
        var definition = _builder.Build();
        Assert.Single(definition.Actions);
        Assert.Equal("TestAction", definition.Actions.First().Name);
    }

    #endregion

    #region WithScheme

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void WithScheme_EmptyName_ThrowsArgumentNullException(string? name)
    {
        // Arrange/Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithScheme(name!, s => { }));
    }

    [Fact]
    public void WithScheme_NullConfigurator_ThrowsArgumentNullException()
    {
        // Arrange
        Action<IInputSchemeBuilder>? configurator = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithScheme("ValidName", configurator!));
    }

    [Fact]
    public void WithScheme_DuplicateName_ThrowsInvalidOperationException()
    {
        // Act & Assert
        _builder.WithScheme("MyScheme", s => { });
        Assert.Throws<InvalidOperationException>(() => _builder.WithScheme("MyScheme", s => { }));
    }

    [Fact]
    public void WithScheme_CaseInsensitiveName_DuplicateName_ThrowsInvalidOperationException()
    {
        // Act & Assert
        _builder.WithScheme("MyScheme", s => { });
        Assert.Throws<InvalidOperationException>(() => _builder.WithScheme("MyScHeMe", s => { }));
    }

    [Fact]
    public void WithScheme_ValidScheme_CreatesScheme()
    {
        // Act
        _builder.WithScheme("MyScheme", schemeBuilder =>
        {
            schemeBuilder.WithDevice(new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test"),
                InputMaps = Array.Empty<InputActionMap>()
            });
        });

        // Assert
        var schemes = _builder.GetInputSchemes().ToList();
        Assert.Single(schemes);
        Assert.Equal("MyScheme", schemes[0].Name);
    }

    #endregion

    #region Build

    [Fact]
    public void Build_Default_ReturnsExpectedDefinition()
    {
        // Act
        var definition = _builder.Build();

        // Assert
        Assert.Equal("TestDefinition", definition.Name);
        Assert.Empty(definition.Actions);
        Assert.False(definition.IsDefault);
    }

    [Fact]
    public void Build_MultipleActions_AllAdded()
    {
        // Arrange
        var action1 = new InputAction("Action1", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var action2 = new InputAction("Action2", new HashSet<InputPhase> { InputPhase.End }, ctx => { });

        _builder.WithAction(action1);
        _builder.WithAction(action2);
        _builder.MakeDefault();

        // Act
        var definition = _builder.Build();

        // Assert
        Assert.Equal(2, definition.Actions.Count());
        Assert.True(definition.IsDefault);
    }

    #endregion
}