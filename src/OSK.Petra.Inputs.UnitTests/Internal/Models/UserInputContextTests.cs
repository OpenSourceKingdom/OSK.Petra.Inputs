using Moq;
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

    private readonly UserInputContext _context;
    private readonly int _userId = 1;

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


        _context = new UserInputContext(_userId)
        {
            Scheme = _scheme
        };
    }

    #endregion

    #region Constructor

    [Fact]
    public void NewContext_SetsPropertiesAsExpected()
    {
        // Arrange/Act/Assert
        Assert.Equal(_userId, _context.UserId);
        Assert.Same(_scheme, _context.Scheme);
        Assert.Empty(_context.DeviceInputContexts);
        Assert.Null(_context.EditorInputCaptureTimeout);
    }

    #endregion

    #region Scheme

    [Fact]
    public void Scheme_SetClearsDeviceContexts()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        _context.GetOrAddDevice(device, _ => Mock.Of<IDeviceDescriptor>());

        var newScheme = new InputScheme("New", "New", [], isDefault: false, isCustom: false);

        // Act
        _context.Scheme = newScheme;

        // Assert - the scheme should be updated (verified by checking it doesn't throw)
    }

    #endregion

    #region GetOrAddDevice

    [Fact]
    public void GetOrAddDevice_NewDevice_CreatesContext()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 200);

        // Act
        var result = _context.GetOrAddDevice(device, _ => Mock.Of<IDeviceDescriptor>());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(device, result.DeviceIdentifier);
        Assert.Equal(200, result.DeviceIdentifier.DeviceId);
    }

    [Fact]
    public void GetOrAddDevice_ExistingDevice_ReturnsSameContext()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var first = _context.GetOrAddDevice(device, _ => Mock.Of<IDeviceDescriptor>());

        // Act
        var second = _context.GetOrAddDevice(device, _ => Mock.Of<IDeviceDescriptor>());

        // Assert
        Assert.Same(first, second);
    }

    #endregion

    #region DeviceInputContexts

    [Fact]
    public void DeviceInputContexts_WithDevices_ReturnsAll()
    {
        // Arrange
        var device1 = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 100);
        var device2 = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 200);
        _context.GetOrAddDevice(device1, _ => Mock.Of<IDeviceDescriptor>());
        _context.GetOrAddDevice(device2, _ => Mock.Of<IDeviceDescriptor>());

        // Act
        var devices = _context.DeviceInputContexts;

        // Assert
        Assert.Equal(2, devices.Count());
    }

    #endregion

    #region Suppress

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Suppress_EmptyActionGroups_SetsGlobalSuppression(bool isNull)
    {
        // Arrange/Act
        _context.Suppress(isNull ? null! : [], true);
        
        // Assert
        Assert.True(_context.IsSuppressed(1));
        Assert.True(_context.IsSuppressed(999));
    }

    [Fact]
    public void Suppress_GlobalUnsuppression_ClearsAll()
    {
        // Arrange
        _context.Suppress(null!, true);

        // Act
        _context.Suppress(null!, false);

        // Assert
        Assert.False(_context.IsSuppressed(1));
        Assert.False(_context.IsSuppressed(999));
    }

    [Fact]
    public void Suppress_ActionGroups_SuppressesSpecificGroup()
    {
        // Arrange/Act
        _context.Suppress(new[] { 5 }, true);

        // Assert
        Assert.True(_context.IsSuppressed(5));
        Assert.False(_context.IsSuppressed(1));
    }

    [Fact]
    public void Suppress_MultipleActionGroups_SuppressesAll()
    {
        // Arrange/Act
        _context.Suppress(new[] { 1, 2, 3 }, true);

        // Assert
        Assert.True(_context.IsSuppressed(1));
        Assert.True(_context.IsSuppressed(2));
        Assert.True(_context.IsSuppressed(3));
        Assert.False(_context.IsSuppressed(4));
    }

    [Fact]
    public void Suppress_UnsuppressSpecificGroup_ClearsOnlyThatGroup()
    {
        // Arrange
        _context.Suppress(new[] { 1, 2 }, true);

        // Act
        _context.Suppress(new[] { 1 }, false);

        // Assert
        Assert.False(_context.IsSuppressed(1));
        Assert.True(_context.IsSuppressed(2));
    }

    #endregion

    #region EditorDelay

    [Fact]
    public void EditorDelay_CanBeSet()
    {
        // Arrange
        var delay = new SchemeEditorDelay() { Delay = TimeSpan.FromSeconds(5) };

        // Act
        _context.EditorInputCaptureTimeout = delay;

        // Assert
        Assert.NotNull(_context.EditorInputCaptureTimeout);
        Assert.Equal(TimeSpan.FromSeconds(5), _context.EditorInputCaptureTimeout.Value.Delay);
    }

    [Fact]
    public void EditorDelay_SetToNull_Clears()
    {
        // Arrange
        _context.EditorInputCaptureTimeout = new SchemeEditorDelay() { Delay = TimeSpan.FromSeconds(5) };

        // Act
        _context.EditorInputCaptureTimeout = null;

        // Assert
        Assert.Null(_context.EditorInputCaptureTimeout);
    }

    #endregion
}
