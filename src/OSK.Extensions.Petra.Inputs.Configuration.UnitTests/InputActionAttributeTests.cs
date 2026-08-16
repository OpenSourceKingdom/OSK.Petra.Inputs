using OSK.Extensions.Petra.Inputs.Configuration.Attributes;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests;

public class InputActionAttributeTests
{
    #region ActionName_NullByDefault

    [Fact]
    public void ActionName_DefaultValueIsNull()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act & Assert
        Assert.Null(attr.ActionName);
    }

    [Fact]
    public void ActionName_SetToValue_ReturnsValue()
    {
        // Arrange
        var attr = new InputActionAttribute { ActionName = "Custom" };

        // Act & Assert
        Assert.Equal("Custom", attr.ActionName);
    }

    #endregion

    #region ActionName_TrimsWhitespace

    [Fact]
    public void ActionName_SetWithLeadingWhitespace_Trims()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act
        attr.ActionName = "  Custom";

        // Assert
        Assert.Equal("Custom", attr.ActionName);
    }

    [Fact]
    public void ActionName_SetWithTrailingWhitespace_Trims()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act
        attr.ActionName = "Custom  ";

        // Assert
        Assert.Equal("Custom", attr.ActionName);
    }

    [Fact]
    public void ActionName_SetWithBothWhitespace_Trims()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act
        attr.ActionName = "  Custom  ";

        // Assert
        Assert.Equal("Custom", attr.ActionName);
    }

    #endregion

    #region TriggerPhases_EmptyByDefault

    [Fact]
    public void TriggerPhases_DefaultValueIsEmptyArray()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act & Assert
        Assert.Empty(attr.TriggerPhases);
    }

    [Fact]
    public void TriggerPhases_SetToValues_ReturnsValues()
    {
        // Arrange
        var attr = new InputActionAttribute
        {
            TriggerPhases = new[] { InputPhase.Start, InputPhase.End }
        };

        // Act & Assert
        Assert.Equal(2, attr.TriggerPhases.Length);
        Assert.Contains(InputPhase.Start, attr.TriggerPhases);
        Assert.Contains(InputPhase.End, attr.TriggerPhases);
    }

    #endregion

    #region Description_NullByDefault

    [Fact]
    public void Description_DefaultValueIsNull()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act & Assert
        Assert.Null(attr.Description);
    }

    [Fact]
    public void Description_SetToValue_ReturnsValue()
    {
        // Arrange
        var attr = new InputActionAttribute { Description = "My description" };

        // Act & Assert
        Assert.Equal("My description", attr.Description);
    }

    #endregion

    #region ActionGroup_RoundTrip

    [Fact]
    public void ActionGroup_DefaultValueIsZero()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act & Assert
        Assert.Equal(0, attr.ActionGroup);
    }

    [Fact]
    public void ActionGroup_SetToValue_ReturnsSameValue()
    {
        // Arrange
        var attr = new InputActionAttribute { ActionGroup = 42 };

        // Act & Assert
        Assert.Equal(42, attr.ActionGroup);
    }

    [Fact]
    public void ActionGroup_InternalActionGroup_SyncsCorrectly()
    {
        // Arrange
        var attr = new InputActionAttribute();

        // Act
        attr.ActionGroup = 10;

        // Assert
        Assert.Equal(10, attr.InternalActionGroup);
    }

    [Fact]
    public void ActionGroup_InternalActionGroup_GetReturnsStoredValue()
    {
        // Arrange
        var attr = new InputActionAttribute { ActionGroup = 7 };

        // Act & Assert
        Assert.Equal(7, attr.ActionGroup);
    }

    #endregion
}
