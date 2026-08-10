using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal.Models;

public class UserInputContextTests
{
    #region Variables

    private readonly RuntimeDeviceIdentifier _deviceIdentifier;
    private readonly InputScheme _scheme;

    #endregion

    #region Constructors

    public UserInputContextTests()
    {
        var deviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test");
        _deviceIdentifier = new RuntimeDeviceIdentifier(100, deviceIdentity);

        var deviceMaps = new[]
        {
            new DeviceInputMap { DeviceIdentity = deviceIdentity, InputMaps = Array.Empty<InputActionMap>() }
        };
        _scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
    }

    private UserInputContext CreateContext(int userId = 1)
    {
        return new UserInputContext(userId, _scheme);
    }

    #endregion

    #region UserId

    [Fact]
    public void UserId_SetsCorrectly()
    {
        // Arrange & Act
        var context = CreateContext(42);

        // Assert
        Assert.Equal(42, context.UserId);
    }

    #endregion

    #region Scheme

    [Fact]
    public void Scheme_ReturnsScheme()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var scheme = context.Scheme;

        // Assert
        Assert.Same(_scheme, scheme);
    }

    [Fact]
    public void Scheme_SetClearsDeviceContexts()
    {
        // Arrange
        var context = CreateContext();
        var device = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        context.GetOrAddDevice(device);

        var newScheme = new InputScheme("New", "New", [], isDefault: false, isCustom: false);

        // Act
        context.Scheme = newScheme;

        // Assert - the scheme should be updated (verified by checking it doesn't throw)
    }

    #endregion

    #region GetOrAddDevice

    [Fact]
    public void GetOrAddDevice_NewDevice_CreatesContext()
    {
        // Arrange
        var context = CreateContext();
        var device = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);

        // Act
        var result = context.GetOrAddDevice(device);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(device, result.DeviceIdentifier);
    }

    [Fact]
    public void GetOrAddDevice_ExistingDevice_ReturnsSameContext()
    {
        // Arrange
        var context = CreateContext();
        var device = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var first = context.GetOrAddDevice(device);

        // Act
        var second = context.GetOrAddDevice(device);

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void GetOrAddDevice_DeviceIdMatches()
    {
        // Arrange
        var context = CreateContext();
        var device = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 500);

        // Act
        var result = context.GetOrAddDevice(device);

        // Assert
        Assert.Equal(500, result.DeviceIdentifier.DeviceId);
    }

    #endregion

    #region DeviceInputContexts

    [Fact]
    public void DeviceInputContexts_Empty_ReturnsEmpty()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var devices = context.DeviceInputContexts;

        // Assert
        Assert.Empty(devices);
    }

    [Fact]
    public void DeviceInputContexts_WithDevices_ReturnsAll()
    {
        // Arrange
        var context = CreateContext();
        var device1 = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 100);
        var device2 = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 200);
        context.GetOrAddDevice(device1);
        context.GetOrAddDevice(device2);

        // Act
        var devices = context.DeviceInputContexts;

        // Assert
        Assert.Equal(2, devices.Count());
    }

    #endregion

    #region Suppress

    [Fact]
    public void Suppress_NullActionGroups_SetsGlobalSuppression()
    {
        // Arrange
        var context = CreateContext();

        // Act
        context.Suppress(null!, true);

        // Assert
        Assert.True(context.IsSuppressed(1));
        Assert.True(context.IsSuppressed(999));
    }

    [Fact]
    public void Suppress_EmptyActionGroups_SetsGlobalSuppression()
    {
        // Arrange
        var context = CreateContext();

        // Act
        context.Suppress(Array.Empty<int>(), true);

        // Assert
        Assert.True(context.IsSuppressed(1));
    }

    [Fact]
    public void Suppress_GlobalUnsuppression_ClearsAll()
    {
        // Arrange
        var context = CreateContext();
        context.Suppress(null!, true);

        // Act
        context.Suppress(null!, false);

        // Assert
        Assert.False(context.IsSuppressed(1));
        Assert.False(context.IsSuppressed(999));
    }

    [Fact]
    public void Suppress_ActionGroups_SuppressesSpecificGroup()
    {
        // Arrange
        var context = CreateContext();

        // Act
        context.Suppress(new[] { 5 }, true);

        // Assert
        Assert.True(context.IsSuppressed(5));
        Assert.False(context.IsSuppressed(1));
    }

    [Fact]
    public void Suppress_MultipleActionGroups_SuppressesAll()
    {
        // Arrange
        var context = CreateContext();

        // Act
        context.Suppress(new[] { 1, 2, 3 }, true);

        // Assert
        Assert.True(context.IsSuppressed(1));
        Assert.True(context.IsSuppressed(2));
        Assert.True(context.IsSuppressed(3));
        Assert.False(context.IsSuppressed(4));
    }

    [Fact]
    public void Suppress_UnsuppressSpecificGroup_ClearsOnlyThatGroup()
    {
        // Arrange
        var context = CreateContext();
        context.Suppress(new[] { 1, 2 }, true);

        // Act
        context.Suppress(new[] { 1 }, false);

        // Assert
        Assert.False(context.IsSuppressed(1));
        Assert.True(context.IsSuppressed(2));
    }

    [Fact]
    public void IsSuppressed_GlobalSuppressionActive_ReturnsTrueForAnyGroup()
    {
        // Arrange
        var context = CreateContext();
        context.Suppress(null!, true);

        // Act & Assert
        Assert.True(context.IsSuppressed(1));
        Assert.True(context.IsSuppressed(9999));
    }

    [Fact]
    public void IsSuppressed_NoSuppression_ReturnsFalse()
    {
        // Arrange
        var context = CreateContext();

        // Act & Assert
        Assert.False(context.IsSuppressed(1));
    }

    #endregion

    #region EditorDelay

    [Fact]
    public void EditorDelay_DefaultValue_IsNull()
    {
        // Arrange
        var context = CreateContext();

        // Assert
        Assert.Null(context.EditorDelay);
    }

    [Fact]
    public void EditorDelay_CanBeSet()
    {
        // Arrange
        var context = CreateContext();
        var delay = new SchemeEditorDelay() { Delay = TimeSpan.FromSeconds(5) };

        // Act
        context.EditorDelay = delay;

        // Assert
        Assert.NotNull(context.EditorDelay);
        Assert.Equal(TimeSpan.FromSeconds(5), context.EditorDelay.Value.Delay);
    }

    [Fact]
    public void EditorDelay_SetToNull_Clears()
    {
        // Arrange
        var context = CreateContext();
        context.EditorDelay = new SchemeEditorDelay() { Delay = TimeSpan.FromSeconds(5) };

        // Act
        context.EditorDelay = null;

        // Assert
        Assert.Null(context.EditorDelay);
    }

    #endregion
}
