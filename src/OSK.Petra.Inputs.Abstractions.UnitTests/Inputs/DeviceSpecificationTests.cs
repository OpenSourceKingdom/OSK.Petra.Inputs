using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class DeviceSpecificationTests
{
    #region TryGetInput_FirstCall_PopulatesLookup

    [Fact]
    public void TryGetInput_FirstCall_PopulatesLookup()
    {
        // Arrange
        var spec = new TestableDeviceSpecification();

        // Act
        var result = spec.TryGetInput(1, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TryGetInput_FirstCall_ReturnsCorrectInput()
    {
        // Arrange
        var spec = new TestableDeviceSpecification();

        // Act
        var getResult = spec.TryGetInput(1, out var input);

        // Assert
        Assert.True(getResult);
        Assert.NotNull(input);
        Assert.Equal(1, input!.Id);
    }

    [Fact]
    public void TryGetInput_NonExistentId_ReturnsFalse()
    {
        // Arrange
        var spec = new TestableDeviceSpecification();

        // Act
        var result = spec.TryGetInput(99, out _);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryGetInput_ZeroId_ReturnsFalse()
    {
        // Arrange
        var spec = new TestableDeviceSpecification();

        // Act
        var result = spec.TryGetInput(0, out _);

        // Assert
        Assert.False(result);
    }

    #endregion
}
