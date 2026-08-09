using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class DeviceInputMapTests
{
    #region Variables

    private readonly DeviceIdentity _keyboardIdentity = new(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");

    #endregion

    #region Constructor (required properties)

    [Fact]
    public void DeviceIdentity_RequiredProperty_SetsValue()
    {
        // Arrange & Act
        var map = new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] };

        // Assert
        Assert.Equal(_keyboardIdentity, map.DeviceIdentity);
    }

    #endregion

    #region InputMaps

    [Fact]
    public void InputMaps_Setter_PopulatesLookup()
    {
        // Arrange
        var input = new MockInput(1);
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});
        var inputMaps = new List<InputActionMap> { new InputActionMap(action, input) };

        // Act
        var map = new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = inputMaps };

        // Assert
        Assert.Single(map.InputMaps);
    }

    [Fact]
    public void InputMaps_EmptyCollection_ReturnsEmpty()
    {
        // Arrange & Act
        var map = new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] };

        // Assert
        Assert.Empty(map.InputMaps);
    }

    #endregion

    #region GetInputMap

    [Fact]
    public void GetInputMap_ExistingId_ReturnsMap()
    {
        // Arrange
        var input = new MockInput(1);
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});
        var map = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = [new InputActionMap(action, input)]
        };

        // Act
        var result = map.GetInputMap(1);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetInputMap_NonExistentId_ReturnsNull()
    {
        // Arrange
        var input = new MockInput(1);
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});
        var map = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = [new InputActionMap(action, input)]
        };

        // Act
        var result = map.GetInputMap(99);

        // Assert
        Assert.Null(result);
    }

    #endregion
}
