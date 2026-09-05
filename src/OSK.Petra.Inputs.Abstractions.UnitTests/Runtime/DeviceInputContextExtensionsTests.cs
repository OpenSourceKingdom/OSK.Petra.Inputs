using OSK.Petra.Inputs.Abstractions.Runtime;
using Moq;
using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Runtime;

public class DeviceInputContextExtensionsTests
{
    #region Variables

    private readonly Mock<IUserInputContext> _mockContext;

    #endregion

    #region Constructors

    public DeviceInputContextExtensionsTests()
    {
        _mockContext = new Mock<IUserInputContext>();
    }

    #endregion

    #region GetOrCreateFeature_TFeature

    [Fact]
    public void GetOrCreateFeature_NewFeature_ReturnsNewInstance()
    {
        // Arrange
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns((TestCapabilityFeature?)null);

        // Act
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>();

        // Assert
        Assert.NotNull(result);
        _mockContext.Verify(c => c.SetFeature(result), Times.Once);
    }

    [Fact]
    public void GetOrCreateFeature_ExistingFeature_ReturnsCached()
    {
        // Arrange
        var existing = new TestCapabilityFeature();
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns(existing);

        // Act
        _ = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>();
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>();

        // Assert
        Assert.Same(existing, result);
        _mockContext.Verify(c => c.SetFeature(result), Times.Never);
    }

    #endregion

    #region GetOrCreateFeature_TFeature_Factory

    [Fact]
    public void GetOrCreateFeature_Factory_NewFeature_UsesFactory()
    {
        // Arrange
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns((TestCapabilityFeature?)null);
        var item = new TestCapabilityFeature();
        var factory = new Func<TestCapabilityFeature>(() => item);

        // Act
        var result = _mockContext.Object.GetOrCreateFeature(factory);

        // Assert
        Assert.NotNull(result);
        Assert.Same(item, result);
        _mockContext.Verify(c => c.SetFeature(result), Times.Once);
    }

    [Fact]
    public void GetOrCreateFeature_Factory_ExistingFeature_ReturnsCached()
    {
        // Arrange
        var existing = new TestCapabilityFeature();
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns(existing);
        var factory = new Func<TestCapabilityFeature>(() => throw new ArgumentNullException());

        // Act
        var result = _mockContext.Object.GetOrCreateFeature(factory);

        // Assert
        Assert.Same(existing, result);
    }

    [Fact]
    public void GetOrCreateFeature_Factory_NullFactory_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>(null!));
    }

    #endregion
}
