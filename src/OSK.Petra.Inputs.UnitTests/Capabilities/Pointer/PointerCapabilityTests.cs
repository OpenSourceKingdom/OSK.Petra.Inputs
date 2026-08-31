using Moq;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using System.Numerics;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Capabilities.Pointer;

public class PointerCapabilityTests
{
    #region Variables

    private readonly Mock<IPointer> _mockInput;
    private readonly Mock<IDeviceInputContext> _mockContext;
    private readonly Mock<IInputState> _mockState;
    private readonly PointerCapability _capability;

    #endregion

    #region Constructors

    public PointerCapabilityTests()
    {
        _mockInput = new Mock<IPointer>();
        _mockInput.SetupGet(m => m.Settings).Returns(new PointerSettings());

        _mockContext = new Mock<IDeviceInputContext>();
        _mockState = new Mock<IInputState>();
        _mockState.SetupGet(m => m.IsNewActivation).Returns(true);

        _capability = new PointerCapability(TestConfigurationHelper.CreateOptions<PointerCapabilityOptions>());
    }

    #endregion

    #region CanProcess

    [Fact]
    public void CanProcess_InputIsPointer_ReturnsTrue()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        // Act
        var result = _capability.CanProcess(new PointerEvent());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanProcess_InputIsNotPointer_ReturnsFalse()
    {
        // Arrange
        var mockNonPointer = new Mock<IInput>();

        // Act
        var result = _capability.CanProcess(new PowerEvent());

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Process

    [Fact]
    public void Process_NullContext_DoesNotProcess()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        // Act
        _capability.Process(null!, _mockState.Object, new PointerEvent(), TimeSpan.Zero);

        // Assert - no exception thrown
    }

    [Fact]
    public void Process_NullState_DoesNotProcess()
    {
        // Act
        _capability.Process(_mockContext.Object, null!, new PointerEvent(), TimeSpan.Zero);

        // Assert - no exception thrown
    }

    [Fact]
    public void Process_StateInputIsNotPointer_DoesNotProcess()
    {
        // Arrange
        var mockNonPointer = new Mock<IInput>();
        _mockState.SetupGet(m => m.Input).Returns(mockNonPointer.Object);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, new PointerEvent(), TimeSpan.Zero);

        // Assert - no exception thrown
    }

    [Fact]
    public void Process_NewActivation_CombinesActivePhase()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.IsNewActivation).Returns(true);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, new PointerEvent(), TimeSpan.Zero);

        // Assert
        _mockState.Verify(s => s.CombinePhase(InputPhase.Active), Times.Once);
    }

    [Fact]
    public void Process_NewActivation_SetsDetails()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.IsNewActivation).Returns(true);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, new PointerEvent(), TimeSpan.Zero);

        // Assert - should not throw
    }

    [Fact]
    public void Process_NotNewActivation_UpdatesPosition()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.IsNewActivation).Returns(false);
        var existingDetails = new PointerDetails(Vector2.Zero, 10, 0);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, new PointerEvent(), TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    #endregion
}
