using Moq;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.UnitTests.Capabilities.Power;

public class PowerCombinationInputTests
{
    #region InputIdentifiers

    [Fact]
    public void InputIdentifiers_ReturnsExpectedOrder()
    {
        // Arrange
        IEnumerable<DeviceInputIdentifier> identifiers = [new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 2)];
        var combinationInput = new PowerCombinationInput(identifiers);

        // Act/Assert
        Assert.True(combinationInput.InputIdentifiers.SequenceEqual(identifiers));
    }

    [Fact]
    public void InputIdentifiers_DuplicateIdentifiers_ReturnsExpectedOrderWithoutDuplicates()
    {
        // Arrange
        IEnumerable<DeviceInputIdentifier> expectedIdentifiers = [new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 2)];
        var combinationInput = new PowerCombinationInput(expectedIdentifiers.Concat(expectedIdentifiers));

        // Act/Asser
        Assert.Equal(2, combinationInput.InputIdentifiers.Count);
        Assert.True(combinationInput.InputIdentifiers.SequenceEqual(expectedIdentifiers));
    }

    #endregion

    #region Matches

    [Fact]
    public void Matches_NonCombinationInput_ReturnsFalse()
    {
        // Arrange
        var combinationInput = new PowerCombinationInput([new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 2)]);
        var otherInput = new Mock<IPointer>();

        // Act
        var result = combinationInput.Equals(otherInput.Object);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Matches_CombinationInput_NotMatchSequence_ReturnsFalse()
    {
        // Arrange
        var combinationInput = new PowerCombinationInput([new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 2)]);
        var combinationInput2 = new PowerCombinationInput([new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 4)]);
        var combinationInput3 = new PowerCombinationInput([new(DeviceIdentities.GenericGamepad, 1), new(DeviceIdentities.GenericGamepad, 2)]);

        // Act
        var result1 = combinationInput.Equals(combinationInput2);
        var result2 = combinationInput.Equals(combinationInput3);

        // Assert
        Assert.False(result1);
        Assert.False(result2);
    }

    [Fact]
    public void Matches_CombinationInput_MatchSequences_VariousOrdering_ReturnsTrue()
    {
        // Arrange
        var combinationInput = new PowerCombinationInput([new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 2)]);
        var combinationInput2 = new PowerCombinationInput([new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 2)]);
        var combinationInput3 = new PowerCombinationInput([new(DeviceIdentities.GenericKeyboard, 2), new(DeviceIdentities.GenericKeyboard, 1)]);

        // Act
        var result1 = combinationInput.Equals(combinationInput2);
        var result2 = combinationInput.Equals(combinationInput3);

        // Assert
        Assert.True(result1);
        Assert.True(result2);
    }

    #endregion
}
