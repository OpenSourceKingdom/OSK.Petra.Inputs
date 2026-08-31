using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class InputActionTests
{
    #region Constructor

    [Fact]
    public void Constructor_AllPropertiesSet_SetsPropertiesAsExpected()
    {
        // Arrange & Act
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {}, description: "Hello", actionGroup: 1);

        // Assert
        Assert.Equal("Move", action.Name);
        Assert.Single(action.TriggerPhases);
        Assert.Contains(InputPhase.Start, action.TriggerPhases);
        Assert.NotNull(action.ActionExecutor);
        Assert.Equal("Hello", action.Description);
        Assert.Equal(1, action.ActionGroup);
    }

    [Fact]
    public void Constructor_SomePropertiesNotSet_SetsPropertiesAsExpected()
    {
        // Arrange & Act
        var phases = new HashSet<InputPhase> { InputPhase.Start, InputPhase.Active };
        var action = new InputAction("Move", phases, ctx => {});

        // Assert
        Assert.Equal("Move", action.Name);
        Assert.Equal(2, action.TriggerPhases.Count);
        Assert.True(phases.SequenceEqual(action.TriggerPhases));
        Assert.Contains(InputPhase.Start, action.TriggerPhases);
        Assert.NotNull(action.ActionExecutor);
        Assert.Null(action.Description);
        Assert.Null(action.ActionGroup);
    }

    #endregion
}
