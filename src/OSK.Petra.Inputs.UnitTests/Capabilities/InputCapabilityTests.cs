using Moq;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Capabilities;

public class InputCapabilityTests
{
    #region Variables

    private readonly Mock<IDeviceInputContext> _mockContext;
    private readonly Mock<IInputState> _mockState;
    private readonly TestableInputCapability _capability;

    public InputCapabilityTests()
    {
        _mockContext = new Mock<IDeviceInputContext>();
        _mockState = new Mock<IInputState>();
        _capability = new TestableInputCapability();
    }

    #endregion

    #region CanProcess

    [Fact]
    public void CanProcess_InputIsTInput_ReturnsTrue()
    {
        // Arrange
        var input = new PowerEvent();

        // Act
        var result = _capability.CanProcess(input);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanProcess_InputIsNotTInput_ReturnsFalse()
    {
        // Arrange
        var differentInput = new PowerEvent();

        // Act
        var result = _capability.CanProcess(differentInput);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Process

    [Fact]
    public void Process_ContextNull_DoesNotCallAbstractProcess()
    {
        // Arrange
        _mockState.Setup(s => s.Input).Returns(new MockInput(1));

        // Act
        _capability.Process(null!, _mockState.Object, new PowerEvent(), TimeSpan.Zero);

        // Assert
        Assert.False(_capability.AbstractProcessCalled);
    }

    [Fact]
    public void Process_StateNull_DoesNotCallAbstractProcess()
    {
        // Arrange
        var input = new MockInput(1);

        // Act
        _capability.Process(_mockContext.Object, null!, new PowerEvent(), TimeSpan.Zero);

        // Assert
        Assert.False(_capability.AbstractProcessCalled);
    }

    [Fact]
    public void Process_StateInputNotTInput_DoesNotCallAbstractProcess()
    {
        // Arrange
        _mockState.Setup(s => s.Input).Returns(Mock.Of<IInput>());

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, new PointerEvent(), TimeSpan.Zero);

        // Assert
        Assert.False(_capability.AbstractProcessCalled);
    }

    [Fact]
    public void Process_ValidInputs_CallsAbstractProcess()
    {
        // Arrange
        var input = new MockInput(1);
        _mockState.Setup(s => s.Input).Returns(input);
        var expectedDelta = TimeSpan.FromSeconds(2.5);


        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, new PowerEvent(), expectedDelta);

        // Assert
        Assert.True(_capability.AbstractProcessCalled);
        Assert.Equal(expectedDelta, _capability.ReceivedDeltaTime);
    }

    #endregion
}
