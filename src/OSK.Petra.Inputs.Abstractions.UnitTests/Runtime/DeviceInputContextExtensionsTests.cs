using OSK.Petra.Inputs.Abstractions.Runtime;
using Moq;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class DeviceInputContextExtensionsTests
{
    #region Variables

    private readonly Mock<IDeviceInputContext> _mockContext;

    public DeviceInputContextExtensionsTests()
    {
        _mockContext = new Mock<IDeviceInputContext>();
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
    }

    [Fact]
    public void GetOrCreateFeature_NewFeature_CallsSetFeature()
    {
        // Arrange
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns((TestCapabilityFeature?)null);

        // Act
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>();

        // Assert
        _mockContext.Verify(c => c.SetFeature(result), Times.Once);
    }

    [Fact]
    public void GetOrCreateFeature_ExistingFeature_ReturnsCached()
    {
        // Arrange
        var existing = new TestCapabilityFeature();
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns(existing);

        // Act
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>();

        // Assert
        Assert.Same(existing, result);
    }

    [Fact]
    public void GetOrCreateFeature_ExistingFeature_DoesNotCallSetFeature()
    {
        // Arrange
        var existing = new TestCapabilityFeature();
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns(existing);

        // Act
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>();

        // Assert
        _mockContext.Verify(c => c.SetFeature(It.IsAny<TestCapabilityFeature>()), Times.Never);
    }

    #endregion

    #region GetOrCreateFeature_TFeature_Factory

    [Fact]
    public void GetOrCreateFeature_Factory_NewFeature_UsesFactory()
    {
        // Arrange
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns((TestCapabilityFeature?)null);
        var factory = new Func<TestCapabilityFeature>(() => new TestCapabilityFeature());

        // Act
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>(factory);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetOrCreateFeature_Factory_NewFeature_CallsSetFeature()
    {
        // Arrange
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns((TestCapabilityFeature?)null);
        var factory = new Func<TestCapabilityFeature>(() => new TestCapabilityFeature());

        // Act
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>(factory);

        // Assert
        _mockContext.Verify(c => c.SetFeature(result), Times.Once);
    }

    [Fact]
    public void GetOrCreateFeature_Factory_ExistingFeature_ReturnsCached()
    {
        // Arrange
        var existing = new TestCapabilityFeature();
        _mockContext.Setup(c => c.GetFeature<TestCapabilityFeature>()).Returns(existing);
        var factory = new Func<TestCapabilityFeature>(() => new TestCapabilityFeature());

        // Act
        var result = _mockContext.Object.GetOrCreateFeature<TestCapabilityFeature>(factory);

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

    #region Helper Types

    private class TestCapabilityFeature : ICapabilityFeature { }

    #endregion
}
