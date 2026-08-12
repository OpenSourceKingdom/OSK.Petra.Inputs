using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class InputPhaseExtensionsTests
{
    #region Combine

    [Theory]
    [InlineData(InputPhase.Start, InputPhase.Start, InputPhase.Start)]
    [InlineData(InputPhase.Start, InputPhase.Active, InputPhase.Start)]
    [InlineData(InputPhase.Start, InputPhase.End, InputPhase.End)]

    [InlineData(InputPhase.End, InputPhase.Start, InputPhase.End)]
    [InlineData(InputPhase.End, InputPhase.Active, InputPhase.End)]
    [InlineData(InputPhase.End, InputPhase.End, InputPhase.End)]

    [InlineData(InputPhase.Active, InputPhase.Start, InputPhase.Start)]
    [InlineData(InputPhase.Active, InputPhase.Active, InputPhase.Active)]
    [InlineData(InputPhase.Active, InputPhase.End, InputPhase.End)]
    public void Combine_PhaseVariations_ReturnsExpectedPhase(InputPhase phase1, InputPhase phase2, InputPhase expectedPhase)
    {
        // Arrange/Act
        var actualPhase = phase1.Combine(phase2);

        // Assert
        Assert.Equal(expectedPhase, actualPhase);
    }

    #endregion
}
