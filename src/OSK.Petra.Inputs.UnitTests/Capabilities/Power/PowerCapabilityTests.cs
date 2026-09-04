using Microsoft.Extensions.Logging;
using Moq;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Capabilities.Power;

public class PowerCapabilityTests
{
    #region Variables

    private readonly Mock<IPowerInput> _mockInput;
    private readonly Mock<IUserInputContext> _mockContext;
    private readonly Mock<IInputState> _mockState;
    private readonly PowerCapability _capability;

    #endregion

    #region Constructors

    public PowerCapabilityTests()
    {
        _mockInput = new Mock<IPowerInput>();
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());

        _mockContext = new Mock<IUserInputContext>();
        _mockState = new Mock<IInputState>();
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Duration).Returns(TimeSpan.FromSeconds(1));
        _mockState.SetupGet(m => m.IsNewActivation).Returns(false);

        _capability = new PowerCapability(TestConfigurationHelper.CreateOptions<PowerCapabilityOptions>(), Mock.Of<ILogger<PowerCapability>>());
    }

    #endregion

    #region CanProcess

    [Fact]
    public void CanProcess_InputIsPower_ReturnsTrue()
    {
        // Arrange/Act
        var result = _capability.CanProcess(new PowerEvent());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanProcess_InputIsNotPower_ReturnsFalse()
    {
        // Arrange/Act
        var result = _capability.CanProcess(new PointerEvent());

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Process

    [Fact]
    public void Process_NullContext_DoesNotProcess()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input)
            .Returns(_mockInput.Object);

        // Act
        _capability.Process(null!, _mockState.Object, new PowerEvent(), TimeSpan.Zero);

        // Assert
        _mockState.Verify(m => m.Input, Times.Never);
    }

    [Fact]
    public void Process_NullState_DoesNotProcess()
    {
        // Act
        _capability.Process(_mockContext.Object, null!, new PowerEvent(), TimeSpan.Zero);

        // Assert

        // Power capability calls a function on the state directly if ran, so no exception validates success
    }

    [Fact]
    public void Process_InputEventIsNotPowerEvent_DoesNotProcess()
    {
        // Arrange
        var mockNonPower = new Mock<IDeviceInput>();
        _mockState.SetupGet(m => m.Input).Returns(mockNonPower.Object);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, new PointerEvent(), TimeSpan.Zero);

        // Assert
        _mockState.Verify(m => m.Input, Times.Never);
    }

    [Fact]
    public void Process_EndPhase_AllowReactivation_DisposesWhenTimeExceeded()
    {
        // Arrange
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings { AllowReactivation = true });

        var options = new PowerCapabilityOptions { ReactivationTime = TimeSpan.FromSeconds(1) };
        var capability = new PowerCapability(TestConfigurationHelper.CreateOptions(options), Mock.Of<ILogger<PowerCapability>>());

        var existingDetails = new PowerDetails()
        {
            Axis = PowerAxis.X,
            Power = 0.5,
            TimeSinceLastActivation = TimeSpan.FromSeconds(1),
            ActivationCount = 0
        };
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.End);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.Setup(m => m.GetDetails<PowerDetails>())
            .Returns(existingDetails);

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, new PowerEvent(PowerAxis.Neutral, .1f), TimeSpan.FromSeconds(2));

        // Assert
        _mockState.Verify(m => m.Dispose(), Times.Once);
    }

    [Fact]
    public void Process_EndPhase_NoReactivation_Disposes()
    {
        // Arrange
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings { AllowReactivation = false });

        var capability = new PowerCapability(TestConfigurationHelper.CreateOptions<PowerCapabilityOptions>(), Mock.Of<ILogger<PowerCapability>>());

        var existingDetails = new PowerDetails()
        {
            Axis = PowerAxis.X,
            Power = 0.5,
            TimeSinceLastActivation = TimeSpan.Zero,
            ActivationCount = 0
        };
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.End);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.Setup(m => m.GetDetails<PowerDetails>())
            .Returns(existingDetails);

        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.End);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, new PowerEvent(PowerAxis.One, .1f), TimeSpan.FromSeconds(1));

        // Assert
        _mockState.Verify(m => m.Dispose(), Times.Once);
    }

    [Fact]
    public void Process_PowerAboveThreshold_StartPhase_WhenBelowActiveThreshold()
    {
        // Arrange
        var options = new PowerCapabilityOptions { ActiveTimeThreshold = TimeSpan.FromSeconds(10) };
        var capability = new PowerCapability(TestConfigurationHelper.CreateOptions(options), Mock.Of<ILogger<PowerCapability>>());

        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.Duration).Returns(TimeSpan.FromSeconds(1));

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, new PowerEvent(PowerAxis.One, 1), TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    [Fact]
    public void Process_PowerAboveThreshold_ActivePhase_WhenAboveActiveThreshold()
    {
        // Arrange
        var options = new PowerCapabilityOptions { ActiveTimeThreshold = TimeSpan.FromSeconds(0.5) };
        var capability = new PowerCapability(TestConfigurationHelper.CreateOptions(options), Mock.Of<ILogger<PowerCapability>>());

        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.Duration).Returns(TimeSpan.FromSeconds(1));

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, new PowerEvent(PowerAxis.One, 1), TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    [Fact]
    public void Process_PowerBelowThreshold_SetsPhaseToEnd()
    {
        // Arrange
        var options = new PowerCapabilityOptions();
        var capability = new PowerCapability(TestConfigurationHelper.CreateOptions(options), Mock.Of<ILogger<PowerCapability>>());

        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        _mockInput.Setup(m => m.Settings)
            .Returns(new PowerSettings()
            {
                PowerSensitivityThreshold = 0.5f
            });

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, new PowerEvent(PowerAxis.One, .1f), TimeSpan.FromSeconds(1));

        // Assert
        _mockState.Verify(m => m.CombinePhase(It.Is<InputPhase>(p => p == InputPhase.End)), Times.Once);
    }

    [Fact]
    public void Process_CombinationInput_AllBaseInputsActive_TriggersCombination()
    {
        // Arrange
        var options = new PowerCapabilityOptions();
        var capability = new PowerCapability(TestConfigurationHelper.CreateOptions(options), Mock.Of<ILogger<PowerCapability>>());

        var combinationInput = new PowerCombinationInput([new(DeviceIdentities.GenericKeyboard, 1), new(DeviceIdentities.GenericKeyboard, 2)]);

        var mockDeviceInput = new Mock<IDeviceInput>();
        mockDeviceInput.SetupGet(m => m.Id)
            .Returns(1);
        IInputState? previousState = new DeviceInputState(new(new(1, DeviceIdentities.GenericKeyboard), Mock.Of<IDeviceDescriptor>()), mockDeviceInput.Object);

        _mockContext
            .SetupSequence(m => m.TryGetInputState(
                It.IsAny<DeviceIdentity>(),
                1,
                out previousState))
            .Returns(true);

        var mockVirtualContext = new Mock<IVirtualInputContext>();
        mockVirtualContext.Setup(m => m.GetInputs<IPowerCombinationInput>())
            .Returns([combinationInput]);
        mockVirtualContext.Setup(m => m.GetOrCreateState(It.IsAny<IVirtualInput>(), It.IsAny<Func<IInputEvent[]>>()))
            .Returns(Mock.Of<IInputState>());

        _mockContext.Setup(m => m.VirtualInputContext)
            .Returns(mockVirtualContext.Object);


        var mockDeviceInput2 = new Mock<IDeviceInput>();
        mockDeviceInput2.SetupGet(m => m.Id)
            .Returns(2);
        var newState = new DeviceInputState(new(new(2, DeviceIdentities.GenericKeyboard), Mock.Of<IDeviceDescriptor>()), mockDeviceInput2.Object);

        // Act
        capability.Process(_mockContext.Object, newState, new PowerEvent(PowerAxis.One, .1f), TimeSpan.FromSeconds(1));

        // Assert
        mockVirtualContext.Verify(m => m.GetOrCreateState(It.IsAny<IVirtualInput>(), It.IsAny<Func<IInputEvent[]>>()), Times.Once);
    }

    #endregion
}
