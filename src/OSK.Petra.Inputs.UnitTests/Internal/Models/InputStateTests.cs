using Moq;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal.Models;

public class InputStateTests
{
    #region Variables

    private readonly Mock<IInput> _mockInput;
    private readonly RuntimeDeviceIdentifier _deviceIdentifier;

    private InputState _state;

    #endregion

    #region Constructors

    public InputStateTests()
    {
        _mockInput = new Mock<IInput>();
        _mockInput.SetupGet(m => m.Id).Returns(1);
        var deviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test");
        _deviceIdentifier = new RuntimeDeviceIdentifier(100, deviceIdentity);


        var deviceContext = new DeviceInputContext(1, _deviceIdentifier);
        _state = new InputState(_mockInput.Object, deviceContext);
    }

    #endregion

    #region Constructor

    [Fact]
    public void Constructor_New_ReturnsExpected()
    {
        // Arrange/Assert
        Assert.Same(_mockInput.Object, _state.Input);
        Assert.Equal(_deviceIdentifier, _state.DeviceIdentifier);
        Assert.True(_state.IsNewActivation);
        Assert.Equal(InputPhase.Start, _state.Phase);
        Assert.Equal(TimeSpan.Zero, _state.Duration);
    }

    #endregion

    #region Reset

    [Fact]
    public void Reset_SetsIsNewActivationToFalse()
    {
        // Arrange
        Assert.True(_state.IsNewActivation);

        // Act
        _state.Reset();

        // Assert
        Assert.False(_state.IsNewActivation);
    }

    #endregion

    #region Phase

    [Theory]
    [InlineData(InputPhase.Start)]
    [InlineData(InputPhase.Active)]
    [InlineData(InputPhase.End)]
    public void CombinePhase_FirstCombine_SetsPhase(InputPhase phase)
    {
        // Arrange/Act
        _state.CombinePhase(phase);

        // Assert
        Assert.Equal(phase, _state.Phase);
    }

    [Theory]
    // These tests aren't exhaustive as there are tests for the combine phase extension
    [InlineData(InputPhase.Start, InputPhase.Start, InputPhase.Start)]
    [InlineData(InputPhase.Active, InputPhase.Start, InputPhase.Start)]
    [InlineData(InputPhase.End, InputPhase.Start, InputPhase.End)]
    public void CombinePhase_SecondCall_CombinesPhaseToExpected(InputPhase initial, InputPhase newPhase, InputPhase expectedPhase)
    {
        // Arrange
        _state.CombinePhase(initial);

        // Act
        _state.CombinePhase(newPhase);

        // Assert
        Assert.Equal(expectedPhase, _state.Phase);
    }

    #endregion

    #region Duration

    [Fact]
    public void Duration_CanBeSet()
    {
        // Arrange
        var expected = TimeSpan.FromSeconds(5);

        // Act
        _state.Duration = expected;

        // Assert
        Assert.Equal(expected, _state.Duration);
    }

    #endregion

    #region SetDetails

    [Fact]
    public void SetDetails_NullDetail_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _state.SetDetails<ICapabilityDetails>(null!));
    }

    [Fact]
    public void SetDetails_ValidDetail_StoresDetail()
    {
        // Arrange
        var detail = new PrimaryCapabilityDetails();

        // Act
        _state.SetDetails(detail);

        // Assert
        var retrieved = _state.GetDetails<PrimaryCapabilityDetails>();
        Assert.NotNull(retrieved);
        Assert.Same(detail, retrieved);
    }

    [Fact]
    public void SetDetails_OverwritesExistingDetail()
    {
        // Arrange
        var detail1 = new PrimaryCapabilityDetails();
        var detail2 = new PrimaryCapabilityDetails();

        // Act
        _state.SetDetails(detail1);
        _state.SetDetails(detail2);

        // Assert
        var retrieved = _state.GetDetails<PrimaryCapabilityDetails>();
        Assert.Same(detail2, retrieved);
    }

    #endregion

    #region GetDetails

    [Fact]
    public void GetDetails_NoDetail_Stored_ReturnsNull()
    {
        // Arrange/Act
        var result = _state.GetDetails<PrimaryCapabilityDetails>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDetails_DifferentType_ReturnsNull()
    {
        // Arrange
        _state.SetDetails(new PrimaryCapabilityDetails());

        // Act
        var result = _state.GetDetails<SecondaryCapabilityDetails>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDetails_ValidType_ReturnsDetails()
    {
        // Arrange
        var details = new PrimaryCapabilityDetails();
        _state.SetDetails(details);

        // Act
        var result = _state.GetDetails<PrimaryCapabilityDetails>();

        // Assert
        Assert.NotNull(result);
        Assert.Same(details, result);
    }

    #endregion

    #region Dispose

    [Fact]
    public void Dispose_SetsIsDisposed_FiresDisposedEvent()
    {
        // Arrange
        IInputState? firedState = null;
        _state.Disposed += s => { firedState = s; };

        // Act
        _state.Dispose();

        // Assert
        Assert.True(_state.IsDisposed);

        Assert.NotNull(firedState);
        Assert.Same(_state, firedState);
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
}
