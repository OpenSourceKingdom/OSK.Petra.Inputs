using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class InputActionTests
{
    #region Constructor

    [Fact]
    public void Constructor_SetsName()
    {
        // Arrange & Act
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});

        // Assert
        Assert.Equal("Move", action.Name);
    }

    [Fact]
    public void Constructor_SetsTriggerPhases()
    {
        // Arrange & Act
        var phases = new HashSet<InputPhase> { InputPhase.Start, InputPhase.Active };
        var action = new InputAction("Move", phases, ctx => {});

        // Assert
        Assert.Same(phases, action.TriggerPhases);
    }

    [Fact]
    public void Constructor_SetsActionExecutor()
    {
        // Arrange & Act
        Action<IInputEventContext> executor = ctx => {};
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, executor);

        // Assert
        Assert.Same(executor, action.ActionExecutor);
    }

    [Fact]
    public void Constructor_SetsDescription()
    {
        // Arrange & Act
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}, "Moves the cursor");

        // Assert
        Assert.Equal("Moves the cursor", action.Description);
    }

    [Fact]
    public void Constructor_NullDescription_SetsNull()
    {
        // Arrange & Act
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}, null);

        // Assert
        Assert.Null(action.Description);
    }

    [Fact]
    public void Constructor_SetsActionGroup()
    {
        // Arrange & Act
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}, "Moves", 42);

        // Assert
        Assert.Equal(42, action.ActionGroup);
    }

    [Fact]
    public void Constructor_NullActionGroup_SetsNull()
    {
        // Arrange & Act
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}, "Moves", null);

        // Assert
        Assert.Null(action.ActionGroup);
    }

    #endregion
}
