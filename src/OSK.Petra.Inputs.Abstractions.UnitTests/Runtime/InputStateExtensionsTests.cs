using OSK.Petra.Inputs.Abstractions.Runtime;
using Moq;
using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Runtime;

public class InputStateExtensionsTests
{
    #region Variables

    private readonly Mock<IInputState> _mockState;

    #endregion

    #region Constructors

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
        _mockState.Verify(s => s.SetDetails(result), Times.Never);
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
        var result = _mockState.Object.GetOrCreateDetails(factory);

        // Assert
        Assert.NotNull(result);
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
        var result = _mockState.Object.GetOrCreateDetails(factory);

        // Assert
        Assert.Same(existing, result);
        _mockState.Verify(s => s.SetDetails(result), Times.Never);
    }

    [Fact]
    public void GetOrCreateDetails_Factory_NullFactory_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _mockState.Object.GetOrCreateDetails<TestCapabilityDetails>(null!));
    }

    #endregion
}
