using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Configuration;

public class DeviceInputMapTests
{
    #region Variables

    private readonly DeviceIdentity _keyboardIdentity = new(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");

    #endregion

    #region Constructor (required properties)

    [Fact]
    public void Constructor_DeviceIdentityAndEmptyMaps_SetsValuesAsExpected()
    {
        // Arrange & Act
        var map = new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = [] };

        // Assert
        Assert.Equal(_keyboardIdentity, map.DeviceIdentity);
        Assert.Empty(map.InputMaps);
    }

    [Fact]
    public void Constructor_DeviceIdentityAndMaps_SetsValuesAsExpected()
    {
        // Arrange
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});
        var inputMaps = new List<DeviceInputActionMap> { new DeviceInputActionMap(action, 1) };

        // Act
        var map = new DeviceInputMap { DeviceIdentity = _keyboardIdentity, InputMaps = inputMaps };

        // Assert
        Assert.Single(map.InputMaps);
    }

    #endregion

    #region GetInputMap

    [Fact]
    public void GetInputMap_ExistingId_ReturnsMap()
    {
        // Arrange
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});
        var map = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = [new DeviceInputActionMap(action, 1)]
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
        var action = new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => {});
        var map = new DeviceInputMap
        {
            DeviceIdentity = _keyboardIdentity,
            InputMaps = [new DeviceInputActionMap(action, 1)]
        };

        // Act
        var result = map.GetInputMap(99);

        // Assert
        Assert.Null(result);
    }

    #endregion
}
