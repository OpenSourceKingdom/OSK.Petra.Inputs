using Moq;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;

namespace OSK.Petra.Inputs.UnitTests.Internal.Models;

public class InputStateTests
{
    #region Variables

    private readonly Mock<IInput> _mockInput;
    private readonly RuntimeDeviceIdentifier _deviceIdentifier;

    #endregion

    #region Constructors

    public InputStateTests()
    {
        _mockInput = new Mock<IInput>();
        _mockInput.SetupGet(m => m.Id).Returns(1);
        var deviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test");
        _deviceIdentifier = new RuntimeDeviceIdentifier(100, deviceIdentity);
    }

    private InputState CreateState(IInput? input = null)
    {
        var deviceContext = new DeviceInputContext(1, _deviceIdentifier);
        return new InputState(input ?? _mockInput.Object, deviceContext);
    }

    #endregion

    #region Constructor

    [Fact]
    public void Constructor_SetsInput()
    {
        // Arrange
        var state = CreateState();

        // Assert
        Assert.Same(_mockInput.Object, state.Input);
    }

    [Fact]
    public void Constructor_SetsDeviceIdentifier()
    {
        // Arrange
        var state = CreateState();

        // Assert
        Assert.Equal(_deviceIdentifier, state.DeviceIdentifier);
    }

    #endregion

    #region IsNewActivation

    [Fact]
    public void IsNewActivation_DefaultValue_IsFalse()
    {
        // Arrange
        var state = CreateState();

        // Assert
        Assert.True(state.IsNewActivation);
    }

    #endregion

    #region Reset

    public void Reset_SetsIsNewActivationToFalse()
    {
        // Arrange
        var state = CreateState();
        Assert.True(state.IsNewActivation);

        // Act
        state.Reset();

        // Assert
        Assert.False(state.IsNewActivation);
    }

    #endregion

    #region Phase

    [Fact]
    public void Phase_DefaultValue_IsZero()
    {
        // Arrange
        var state = CreateState();

        // Assert
        Assert.Equal(InputPhase.Start, state.Phase);
    }

    [Fact]
    public void CombinePhase_Active_SetsPhaseToActive()
    {
        // Arrange
        var state = CreateState();

        // Act
        state.CombinePhase(InputPhase.Active);

        // Assert
        Assert.Equal(InputPhase.Active, state.Phase);
    }

    [Fact]
    public void CombinePhase_End_SetsPhaseToEnd()
    {
        // Arrange
        var state = CreateState();

        // Act
        state.CombinePhase(InputPhase.End);

        // Assert
        Assert.Equal(InputPhase.End, state.Phase);
    }

    #endregion

    #region Duration

    [Fact]
    public void Duration_DefaultValue_IsZero()
    {
        // Arrange
        var state = CreateState();

        // Assert
        Assert.Equal(TimeSpan.Zero, state.Duration);
    }

    [Fact]
    public void Duration_CanBeSet()
    {
        // Arrange
        var state = CreateState();
        var expected = TimeSpan.FromSeconds(5);

        // Act
        state.Duration = expected;

        // Assert
        Assert.Equal(expected, state.Duration);
    }

    #endregion

    #region SetDetails

    [Fact]
    public void SetDetails_NullDetail_ThrowsArgumentNullException()
    {
        // Arrange
        var state = CreateState();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => state.SetDetails<ICapabilityDetails>(null!));
    }

    [Fact]
    public void SetDetails_ValidDetail_StoresDetail()
    {
        // Arrange
        var state = CreateState();
        var detail = new TestCapabilityDetails();

        // Act
        state.SetDetails(detail);

        // Assert
        var retrieved = state.GetDetails<TestCapabilityDetails>();
        Assert.NotNull(retrieved);
        Assert.Same(detail, retrieved);
    }

    [Fact]
    public void SetDetails_OverwritesExistingDetail()
    {
        // Arrange
        var state = CreateState();
        var detail1 = new TestCapabilityDetails();
        var detail2 = new TestCapabilityDetails();

        // Act
        state.SetDetails(detail1);
        state.SetDetails(detail2);

        // Assert
        var retrieved = state.GetDetails<TestCapabilityDetails>();
        Assert.Same(detail2, retrieved);
    }

    #endregion

    #region GetDetails

    [Fact]
    public void GetDetails_NoDetail_Stored_ReturnsNull()
    {
        // Arrange
        var state = CreateState();

        // Act
        var result = state.GetDetails<TestCapabilityDetails>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDetails_DifferentType_ReturnsNull()
    {
        // Arrange
        var state = CreateState();
        state.SetDetails(new TestCapabilityDetails());

        // Act
        var result = state.GetDetails<OtherCapabilityDetails>();

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Dispose

    [Fact]
    public void Dispose_SetsIsDisposed()
    {
        // Arrange
        var state = CreateState();

        // Act
        state.Dispose();

        // Assert
        Assert.True(state.IsDisposed);
    }

    [Fact]
    public void Disposed_Event_Fired()
    {
        // Arrange
        var state = CreateState();
        IInputState? firedState = null;
        state.Disposed += s => { firedState = s; };

        // Act
        state.Dispose();

        // Assert
        Assert.NotNull(firedState);
        Assert.Same(state, firedState);
    }

    [Fact]
    public void Dispose_RemovesFromDeviceContext()
    {
        // Arrange
        var deviceContext = new DeviceInputContext(1, _deviceIdentifier);
        var state = new InputState(_mockInput.Object, deviceContext);

        // Act
        state.Dispose();

        // Assert
        var snapshot = deviceContext.GetInputStateSnapshot();
        Assert.Empty(snapshot);
    }

    #endregion

    #region Helper Classes

    private class TestCapabilityDetails : ICapabilityDetails { }
    private class OtherCapabilityDetails : ICapabilityDetails { }

    #endregion
}
