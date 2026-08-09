using OSK.Petra.Inputs.Abstractions.Runtime;
using Moq;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class InputStateExtensionsTests
{
    #region Variables

    private readonly Mock<IInputState> _mockState;

    public InputStateExtensionsTests()
    {
        _mockState = new Mock<IInputState>();
    }

    #endregion

    #region GetOrCreateDetails_TDetails

    [Fact]
    public void GetOrCreateDetails_NewDetail_ReturnsNewInstance()
    {
        // Arrange
        _mockState.Setup(s => s.GetDetails<TestCapabilityDetails>()).Returns((TestCapabilityDetails?)null);

        // Act
        var result = _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetOrCreateDetails_NewDetail_CallsSetDetails()
    {
        // Arrange
        _mockState.Setup(s => s.GetDetails<TestCapabilityDetails>()).Returns((TestCapabilityDetails?)null);

        // Act
        var result = _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>();

        // Assert
        _mockState.Verify(s => s.SetDetails(result), Times.Once);
    }

    [Fact]
    public void GetOrCreateDetails_ExistingDetail_ReturnsCached()
    {
        // Arrange
        var existing = new TestCapabilityDetails();
        _mockState.Setup(s => s.GetDetails<TestCapabilityDetails>()).Returns(existing);

        // Act
        var result = _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>();

        // Assert
        Assert.Same(existing, result);
    }

    [Fact]
    public void GetOrCreateDetails_ExistingDetail_DoesNotCallSetDetails()
    {
        // Arrange
        var existing = new TestCapabilityDetails();
        _mockState.Setup(s => s.GetDetails<TestCapabilityDetails>()).Returns(existing);

        // Act
        var result = _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>();

        // Assert
        _mockState.Verify(s => s.SetDetails(It.IsAny<TestCapabilityDetails>()), Times.Never);
    }

    #endregion

    #region GetOrCreateDetails_TDetails_Factory

    [Fact]
    public void GetOrCreateDetails_Factory_NewDetail_UsesFactory()
    {
        // Arrange
        _mockState.Setup(s => s.GetDetails<TestCapabilityDetails>()).Returns((TestCapabilityDetails?)null);
        var factory = new Func<TestCapabilityDetails>(() => new TestCapabilityDetails());

        // Act
        var result = _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>(factory);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetOrCreateDetails_Factory_NewDetail_CallsSetDetails()
    {
        // Arrange
        _mockState.Setup(s => s.GetDetails<TestCapabilityDetails>()).Returns((TestCapabilityDetails?)null);
        var factory = new Func<TestCapabilityDetails>(() => new TestCapabilityDetails());

        // Act
        var result = _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>(factory);

        // Assert
        _mockState.Verify(s => s.SetDetails(result), Times.Once);
    }

    [Fact]
    public void GetOrCreateDetails_Factory_ExistingDetail_ReturnsCached()
    {
        // Arrange
        var existing = new TestCapabilityDetails();
        _mockState.Setup(s => s.GetDetails<TestCapabilityDetails>()).Returns(existing);
        var factory = new Func<TestCapabilityDetails>(() => new TestCapabilityDetails());

        // Act
        var result = _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>(factory);

        // Assert
        Assert.Same(existing, result);
    }

    [Fact]
    public void GetOrCreateDetails_Factory_NullFactory_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>(null!));
    }

    #endregion

    #region Helper Types

    private class TestCapabilityDetails : ICapabilityDetails { }

    #endregion
}
