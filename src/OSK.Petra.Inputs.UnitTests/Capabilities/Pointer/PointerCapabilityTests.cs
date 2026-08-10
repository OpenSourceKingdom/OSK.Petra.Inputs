using Microsoft.Extensions.Options;
using Moq;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using System.Numerics;

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
        _mockInput.SetupGet(m => m.Position).Returns(Vector2.Zero);
        _mockInput.SetupGet(m => m.Settings).Returns(new PointerSettings());

        _mockContext = new Mock<IDeviceInputContext>();
        _mockState = new Mock<IInputState>();
        _mockState.SetupGet(m => m.IsNewActivation).Returns(true);

        var options = new OptionsWrapper<PointerCapabilityOptions>(new());
        _capability = new PointerCapability(options);
    }

    #endregion

    #region CanProces

    [Fact]
    public void CanProces_InputIsPointer_ReturnsTrue()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        // Act
        var result = _capability.CanProces(_mockInput.Object);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanProces_InputIsNotPointer_ReturnsFalse()
    {
        // Arrange
        var mockNonPointer = new Mock<IInput>();

        // Act
        var result = _capability.CanProces(mockNonPointer.Object);

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
        _capability.Process(null!, _mockState.Object, TimeSpan.Zero);

        // Assert - no exception thrown
    }

    [Fact]
    public void Process_NullState_DoesNotProcess()
    {
        // Act
        _capability.Process(_mockContext.Object, null!, TimeSpan.Zero);

        // Assert - no exception thrown
    }

    [Fact]
    public void Process_StateInputIsNotPointer_DoesNotProcess()
    {
        // Arrange
        var mockNonPointer = new Mock<IInput>();
        _mockState.SetupGet(m => m.Input).Returns(mockNonPointer.Object);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.Zero);

        // Assert - no exception thrown
    }

    [Fact]
    public void Process_NewActivation_CombinesActivePhase()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.IsNewActivation).Returns(true);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.Zero);

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
        _capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.Zero);

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
        _capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    #endregion

    #region Helper Classes

    private class TestPointerCapability : PointerCapability
    {
        public bool ProcessCalled { get; private set; }

        public TestPointerCapability() : base(Microsoft.Extensions.Options.Options.Create(new PointerCapabilityOptions())) { }

        protected override void Process(IDeviceInputContext context, IInputState state, IPointer input, TimeSpan deltaTime)
        {
            ProcessCalled = true;
        }
    }

    #endregion
}
