using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Inputs;

public class DeviceSpecificationTests
{
    #region TryGetInput

    [Fact]
    public void TryGetInput_OnceCall_ReturnsExpectedInput()
    {
        // Arrange
        var spec = new TestableDeviceSpecification();

        // Act
        var result = spec.TryGetInput(1, out var input);

        // Assert
        Assert.True(result);
        Assert.NotNull(input);
        Assert.Equal(1, input!.Id);
        Assert.Equal(1, spec.LookupPopulateCount);
    }

    [Fact]
    public void TryGetInput_MultipleCalls_PopulatesLookupOnce()
    {
        // Arrange
        var spec = new TestableDeviceSpecification();

        // Act
        spec.TryGetInput(1, out _);
        spec.TryGetInput(1, out _);
        spec.TryGetInput(1, out _);
        spec.TryGetInput(1, out _);
        spec.TryGetInput(1, out _);

        // Assert
        Assert.Equal(1, spec.LookupPopulateCount);
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
