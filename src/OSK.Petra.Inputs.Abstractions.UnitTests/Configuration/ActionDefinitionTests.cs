using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class ActionDefinitionTests
{
    #region Variables

    private static readonly InputAction[] _actions =
    [
        new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}),
        new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start, InputPhase.End }, ctx => {})
    ];

    #endregion

    #region Constructor

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_SetsValues(bool isDefault)
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", _actions, isDefault: isDefault);

        // Assert
        Assert.Equal("Default", definition.Name);
        Assert.Equal(isDefault, definition.IsDefault);
    }

    #endregion

    #region Actions

    [Fact]
    public void Actions_ReturnsAllActions()
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", _actions, isDefault: true);

        // Assert
        Assert.Equal(2, definition.Actions.Count);
        Assert.True(_actions.Select(a => a.Name).Order().SequenceEqual(definition.Actions.Select(a => a.Name).Order()));
    }

    [Fact]
    public void Actions_EmptyActions_ReturnsEmpty()
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", [], isDefault: true);

        // Assert
        Assert.Empty(definition.Actions);
    }

    [Fact]
    public void Actions_NullActions_ReturnsEmpty()
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", null!, isDefault: true);

        // Assert
        Assert.Empty(definition.Actions);
    }

    #endregion

    #region GetAction

    [Fact]
    public void GetAction_ExistingActionName_ReturnsAction()
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", _actions, isDefault: true);
        var result = definition.GetAction("Move");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Move", result!.Name);
    }

    [Fact]
    public void GetAction_NonExistentActionName_ReturnsNull()
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", _actions, isDefault: true);
        var result = definition.GetAction("Jump");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void GetAction_InvalidName_ReturnsNull(string? name)
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", _actions, isDefault: true);
        var result = definition.GetAction(name!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAction_IsCaseInsensitive()
    {
        // Arrange & Act
        var definition = new ActionDefinition("Default", _actions, isDefault: true);
        var result = definition.GetAction("move");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Move", result!.Name);
    }

    #endregion
}
