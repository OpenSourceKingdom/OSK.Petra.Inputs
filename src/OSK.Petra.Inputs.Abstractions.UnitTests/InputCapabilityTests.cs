using Moq;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

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

    #region CanProces

    [Fact]
    public void CanProces_InputIsTInput_ReturnsTrue()
    {
        // Arrange
        var input = new MockInput(1);

        // Act
        var result = _capability.CanProces(input);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanProces_InputIsNotTInput_ReturnsFalse()
    {
        // Arrange
        var differentInput = new MockInput(2, "Y");

        // Act
        var result = _capability.CanProces(differentInput);

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
        _capability.Process(null!, _mockState.Object, TimeSpan.Zero);

        // Assert
        Assert.False(_capability.AbstractProcessCalled);
    }

    [Fact]
    public void Process_StateNull_DoesNotCallAbstractProcess()
    {
        // Arrange
        var input = new MockInput(1);

        // Act
        _capability.Process(_mockContext.Object, null!, TimeSpan.Zero);

        // Assert
        Assert.False(_capability.AbstractProcessCalled);
    }

    [Fact]
    public void Process_StateInputNotTInput_DoesNotCallAbstractProcess()
    {
        // Arrange
        _mockState.Setup(s => s.Input).Returns(Mock.Of<IInput>());

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.Zero);

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
        _capability.Process(_mockContext.Object, _mockState.Object, expectedDelta);

        // Assert
        Assert.True(_capability.AbstractProcessCalled);
        Assert.Equal(expectedDelta, _capability.ReceivedDeltaTime);
    }

    #endregion
}
