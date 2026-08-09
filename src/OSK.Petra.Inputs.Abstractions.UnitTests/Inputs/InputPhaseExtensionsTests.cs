using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class InputPhaseExtensionsTests
{
    #region Combine

    [Fact]
    public void Combine_StartWithEnd_ReturnsEnd()
    {
        // Arrange & Act
        var result = InputPhase.Start.Combine(InputPhase.End);

        // Assert
        Assert.Equal(InputPhase.End, result);
    }

    [Fact]
    public void Combine_EndWithStart_ReturnsEnd()
    {
        // Arrange & Act
        var result = InputPhase.End.Combine(InputPhase.Start);

        // Assert
        Assert.Equal(InputPhase.End, result);
    }

    [Fact]
    public void Combine_ActiveWithEnd_ReturnsEnd()
    {
        // Arrange & Act
        var result = InputPhase.Active.Combine(InputPhase.End);

        // Assert
        Assert.Equal(InputPhase.End, result);
    }

    [Fact]
    public void Combine_EndWithActive_ReturnsEnd()
    {
        // Arrange & Act
        var result = InputPhase.End.Combine(InputPhase.Active);

        // Assert
        Assert.Equal(InputPhase.End, result);
    }

    [Fact]
    public void Combine_StartWithActive_ReturnsStart()
    {
        // Arrange & Act
        var result = InputPhase.Start.Combine(InputPhase.Active);

        // Assert
        Assert.Equal(InputPhase.Start, result);
    }

    [Fact]
    public void Combine_ActiveWithStart_ReturnsStart()
    {
        // Arrange & Act
        var result = InputPhase.Active.Combine(InputPhase.Start);

        // Assert
        Assert.Equal(InputPhase.Start, result);
    }

    [Fact]
    public void Combine_StartWithStart_ReturnsStart()
    {
        // Arrange & Act
        var result = InputPhase.Start.Combine(InputPhase.Start);

        // Assert
        Assert.Equal(InputPhase.Start, result);
    }

    [Fact]
    public void Combine_ActiveWithActive_ReturnsActive()
    {
        // Arrange & Act
        var result = InputPhase.Active.Combine(InputPhase.Active);

        // Assert
        Assert.Equal(InputPhase.Active, result);
    }

    [Fact]
    public void Combine_EndWithEnd_ReturnsEnd()
    {
        // Arrange & Act
        var result = InputPhase.End.Combine(InputPhase.End);

        // Assert
        Assert.Equal(InputPhase.End, result);
    }

    #endregion
}
