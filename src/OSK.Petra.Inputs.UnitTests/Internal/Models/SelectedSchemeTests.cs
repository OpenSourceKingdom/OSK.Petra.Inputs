using Moq;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Models;

namespace OSK.Petra.Inputs.UnitTests.Internal.Models;

public class SelectedSchemeTests
{
    #region Variables

    private readonly InputAction _testAction;
    private readonly DeviceIdentity _deviceIdentity;

    #endregion

    #region Constructors

    public SelectedSchemeTests()
    {
        _testAction = new InputAction("TestAction", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        _deviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard");
    }

    #endregion

    #region Constructor

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_ReturnsExpectedProperties(bool isIt)
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: isIt, isNew: isIt, isPreferred: isIt);

        // Assert
        // Helper sets the name
        Assert.Equal(isIt ? "New Scheme" : "Default", scheme.Name);
        Assert.Equal(isIt, scheme.IsReadonly);
        Assert.Equal(isIt, scheme.IsNew);
        Assert.Equal(isIt, scheme.IsPreferred);
    }

    #endregion

    #region SetName

    [Fact]
    public void SetName_ValidName_SetsName()
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: false);

        // Act
        var result = scheme.SetName("NewName");

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal("NewName", scheme.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetName_InvalidName_ReturnsInvalidRequest(string name)
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: false);

        // Act
        var result = scheme.SetName(name);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetName_ReadonlyScheme_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: true);

        // Act
        var result = scheme.SetName("NewName");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetName_Valid_ReturrnsSuccessfully()
    {
        // Arrange
        var scheme = CreateScheme();

        // Act
        var result = scheme.SetName("Yes");

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal("Yes", scheme.Name);
    }

    #endregion

    #region MakePreferred

    [Fact]
    public void MakePreferred_SetsPreferred()
    {
        // Arrange
        var scheme = CreateScheme(isPreferred: false);
        Assert.False(scheme.IsPreferred);

        // Act
        scheme.MakePreferred();

        // Assert
        Assert.True(scheme.IsPreferred);
    }

    #endregion

    #region UnpairedActions

    [Fact]
    public void UnpairedActions_NoMappings_ReturnsAllActions()
    {
        // Arrange
        var scheme = CreateScheme();

        // Assert
        Assert.Single(scheme.UnpairedActions);
        Assert.Equal("TestAction", scheme.UnpairedActions[0].Name);
    }

    [Fact]
    public void UnpairedActions_WithMapping_ReturnsEmpty()
    {
        // Arrange
        var scheme = CreateSchemeWithMapping();

        // Assert
        Assert.Empty(scheme.UnpairedActions);
    }

    #endregion

    #region ConfiguredInputMaps

    [Fact]
    public void ConfiguredInputMaps_NoMappings_ReturnsEmpty()
    {
        // Arrange
        var scheme = CreateScheme();

        // Assert
        Assert.Empty(scheme.ConfiguredInputMaps);
    }

    [Fact]
    public void ConfiguredInputMaps_WithMapping_ReturnsMappedActions()
    {
        // Arrange
        var scheme = CreateSchemeWithMapping();

        // Assert
        Assert.Single(scheme.ConfiguredInputMaps);
    }

    #endregion

    #region UnpairedInputs

    [Fact]
    public void UnpairedInputs_NoMappings_ReturnsAllInputs()
    {
        // Arrange
        var scheme = CreateScheme();

        // Assert
        Assert.Single(scheme.UnpairedInputs);
    }

    [Fact]
    public void UnpairedInputs_WithMapping_ReturnsEmpty()
    {
        // Arrange
        var scheme = CreateSchemeWithMapping();

        // Assert
        Assert.Empty(scheme.UnpairedInputs);
    }

    #endregion

    #region SetInputMap

    [Fact]
    public void SetInputMap_UnsupportedDevice_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: false);
        var unsupportedIdentity = new DeviceIdentity(DeviceTopologyName.Gamepad, DeviceFamily.Generic, "Gamepad");

        // Act
        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(2);
        var result = scheme.SetInputMap(unsupportedIdentity, _testAction, mockInput.Object);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetInputMap_NullAction_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: false);
        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(2);

        // Act
        var result = scheme.SetInputMap(_deviceIdentity, null!, mockInput.Object);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetInputMap_NullInput_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: false);

        // Act
        var result = scheme.SetInputMap(_deviceIdentity, _testAction, null!);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetInputMap_ReadonlyScheme_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: true);
        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(2);

        // Act
        var result = scheme.SetInputMap(_deviceIdentity, _testAction, mockInput.Object);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetInputMap_ValidMapping_SetsMap()
    {
        // Arrange
        var scheme = CreateScheme(isReadonly: false);
        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(2);

        // Act
        var result = scheme.SetInputMap(_deviceIdentity, _testAction, mockInput.Object);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void SetInputMap_ActionNameIsNotValid_ReturnsInvalidRequest()
    {
        // Arrange
        var existingAction = new InputAction("Existing", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var scheme = CreateSchemeWithMapping();
        // Replace the action in the scheme with our existing action
        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(3);

        // Act
        var result = scheme.SetInputMap(_deviceIdentity, existingAction, mockInput.Object);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    #endregion

    #region ClearConfiguredMaps

    [Fact]
    public void ClearConfiguredMaps_IsReadonlyScheme_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = CreateSchemeWithMapping(isReadOnly: true);

        Assert.Single(scheme.ConfiguredInputMaps);

        // Act
        var result = scheme.ClearConfiguredMaps();

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Single(scheme.ConfiguredInputMaps);
    }

    [Fact]
    public void ClearConfiguredMaps_IsNotReadonly_ReturnsSuccess()
    {
        // Arrange
        var scheme = CreateSchemeWithMapping(isReadOnly: false);
        Assert.Single(scheme.ConfiguredInputMaps);
        Assert.Empty(scheme.UnpairedActions);
        Assert.Empty(scheme.UnpairedInputs);

        // Act
        var result = scheme.ClearConfiguredMaps();

        // Assert
        Assert.True(result.IsSuccessful);

        Assert.Empty(scheme.ConfiguredInputMaps);
        Assert.Single(scheme.UnpairedActions);
        Assert.Single(scheme.UnpairedInputs);
    }

    #endregion

    #region Helpers

    private SelectedScheme CreateScheme(bool isReadonly = false, bool isNew = false, bool isPreferred = false)
    {
        var availableActions = new[] { _testAction };
        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);

        var availableInputs = new[]
        {
            new DeviceMapPairing<IInput>(_deviceIdentity, new[] { mockInput.Object })
        };

        return new SelectedScheme(
            isNew ? "New Scheme" : "Default",
            isReadonly,
            isPreferred,
            isNew,
            availableActions,
            availableInputs,
            []);
    }

    private SelectedScheme CreateSchemeWithMapping(bool isReadOnly = false)
    {
        var availableActions = new[] { _testAction };
        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);

        var availableInputs = new[]
        {
            new DeviceMapPairing<IInput>(_deviceIdentity, new[] { mockInput.Object })
        };

        var actionMap = new InputActionMap(_testAction, mockInput.Object);
        var deviceMapPairings = new[]
        {
            new DeviceMapPairing<InputActionMap>(_deviceIdentity, new[] { actionMap })
        };

        return new SelectedScheme(
            "Default",
            isReadOnly,
            false,
            false,
            availableActions,
            availableInputs,
            deviceMapPairings);
    }

    #endregion
}
