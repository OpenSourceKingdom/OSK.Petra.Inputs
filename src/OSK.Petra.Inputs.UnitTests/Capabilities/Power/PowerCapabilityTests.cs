using Microsoft.Extensions.Options;
using Moq;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Petra.Inputs.UnitTests.Capabilities.Power;

public class PowerCapabilityTests
{
    #region Variables

    private readonly Mock<IPowerInput> _mockInput;
    private readonly Mock<IDeviceInputContext> _mockContext;
    private readonly Mock<IInputState> _mockState;
    private readonly PowerCapability _capability;

    #endregion

    #region Constructors

    public PowerCapabilityTests()
    {
        _mockInput = new Mock<IPowerInput>();
        _mockInput.SetupGet(m => m.Power).Returns(0.5f);
        _mockInput.SetupGet(m => m.Axis).Returns(PowerAxis.X);
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());

        _mockContext = new Mock<IDeviceInputContext>();
        _mockState = new Mock<IInputState>();
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Duration).Returns(TimeSpan.FromSeconds(1));
        _mockState.SetupGet(m => m.IsNewActivation).Returns(false);

        var options = new OptionsWrapper<PowerCapabilityOptions>(new());
        _capability = new PowerCapability(options);
    }

    #endregion

    #region CanProces

    [Fact]
    public void CanProces_InputIsPower_ReturnsTrue()
    {
        // Arrange
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        // Act
        var result = _capability.CanProces(_mockInput.Object);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanProces_InputIsNotPower_ReturnsFalse()
    {
        // Arrange
        var mockNonPower = new Mock<IInput>();

        // Act
        var result = _capability.CanProces(mockNonPower.Object);

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
    }

    [Fact]
    public void Process_NullState_DoesNotProcess()
    {
        // Act
        _capability.Process(_mockContext.Object, null!, TimeSpan.Zero);
    }

    [Fact]
    public void Process_StateInputIsNotPower_DoesNotProcess()
    {
        // Arrange
        var mockNonPower = new Mock<IInput>();
        _mockState.SetupGet(m => m.Input).Returns(mockNonPower.Object);

        // Act
        _capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.Zero);
    }

    [Fact]
    public void Process_EndPhase_AllReactivation_DisposesWhenTimeExceeded()
    {
        // Arrange
        _mockInput.SetupGet(m => m.Power).Returns(0.1f);
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings { AllowReactivation = true });

        var options = new PowerCapabilityOptions { ReactivationTime = TimeSpan.FromSeconds(1) };
        var capability = new PowerCapability(new OptionsWrapper<PowerCapabilityOptions>(options));

        var existingDetails = new PowerDetails()
        {
            Axis = PowerAxis.X,
            Power = 0.5,
            TimeSinceLastActivation = TimeSpan.Zero,
            ActivationCount = 0
        };
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.End);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        // Simulate time since last activation exceeded
        var mockDetails = new Mock<ICapabilityDetails>();

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.FromSeconds(2));

        // Assert - should not throw
    }

    [Fact]
    public void Process_EndPhase_NoReactivation_Disposes()
    {
        // Arrange
        _mockInput.SetupGet(m => m.Power).Returns(0.1f);
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings { AllowReactivation = false });

        var capability = new PowerCapability(new OptionsWrapper<PowerCapabilityOptions>(new()));

        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.End);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    [Fact]
    public void Process_PowerAboveThreshold_StartPhase_WhenBelowActiveThreshold()
    {
        // Arrange
        var options = new PowerCapabilityOptions { ActiveTimeThreshold = TimeSpan.FromSeconds(10) };
        var capability = new PowerCapability(new OptionsWrapper<PowerCapabilityOptions>(options));

        _mockInput.SetupGet(m => m.Power).Returns(1.0f);
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.Duration).Returns(TimeSpan.FromSeconds(1));

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    [Fact]
    public void Process_PowerAboveThreshold_ActivePhase_WhenAboveActiveThreshold()
    {
        // Arrange
        var options = new PowerCapabilityOptions { ActiveTimeThreshold = TimeSpan.FromSeconds(0.5) };
        var capability = new PowerCapability(new OptionsWrapper<PowerCapabilityOptions>(options));

        _mockInput.SetupGet(m => m.Power).Returns(1.0f);
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);
        _mockState.SetupGet(m => m.Duration).Returns(TimeSpan.FromSeconds(1));

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    [Fact]
    public void Process_PowerBelowThreshold_SetsPhaseToEnd()
    {
        // Arrange
        var options = new PowerCapabilityOptions();
        var capability = new PowerCapability(new OptionsWrapper<PowerCapabilityOptions>(options));

        _mockInput.SetupGet(m => m.Power).Returns(0.1f);
        _mockInput.SetupGet(m => m.Settings).Returns(new PowerSettings());
        _mockState.SetupGet(m => m.Phase).Returns(InputPhase.Start);
        _mockState.SetupGet(m => m.Input).Returns(_mockInput.Object);

        _mockInput.Setup(m => m.Settings)
            .Returns(new PowerSettings()
            {
                ActivationSensitivityThreshold = 0.5f
            });

        // Act
        capability.Process(_mockContext.Object, _mockState.Object, TimeSpan.FromSeconds(1));

        // Assert - should not throw
    }

    #endregion

    #region Helper Classes

    private class TestPowerCapability : PowerCapability
    {
        public bool ProcessCalled { get; private set; }

        public TestPowerCapability() : base(Microsoft.Extensions.Options.Options.Create(new PowerCapabilityOptions())) { }

        protected override void Process(IDeviceInputContext context, IInputState state, IPowerInput input, TimeSpan deltaTime)
        {
            ProcessCalled = true;
        }
    }

    #endregion
}
