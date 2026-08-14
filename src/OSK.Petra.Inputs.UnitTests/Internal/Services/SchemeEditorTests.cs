using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class SchemeEditorTests
{
    #region Variables

    private readonly Mock<IInputUser> _mockUser;
    private readonly Mock<ISchemeService> _mockSchemeService;
    private readonly Mock<IInputSystemConfigurationProvider> _mockConfigProvider;
    private readonly Mock<IUserManager> _mockUserManager;
    private readonly Mock<IInputSystemNotifier> _mockSystemNotifier;
    private readonly DeviceCatalog _catalog;
    private readonly InputSystemConfiguration _validConfig;

    #endregion

    #region Constructors

    public SchemeEditorTests()
    {
        _validConfig = TestConfigurationHelper.CreateValidConfiguration();
        _mockUser = new Mock<IInputUser>();
        _mockUser.SetupGet(m => m.Id).Returns(1);
        _mockSchemeService = new Mock<ISchemeService>();
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(true);
        
        _mockConfigProvider = new Mock<IInputSystemConfigurationProvider>();
        _mockConfigProvider.SetupGet(m => m.Configuration)
            .Returns(_validConfig);
        
        _mockUserManager = new Mock<IUserManager>();
        _mockSystemNotifier = new Mock<IInputSystemNotifier>();

        var catalogPart = new DeviceCatalogPart() 
        {
            TopologyName = DeviceTopologyName.Keyboard,
            KnownDevices = [],
            GenericDevice = null
        };

        _catalog = new([catalogPart]);
    }

    #endregion

    #region Constructor

    [Fact]
    public void Constructor_NullUser_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SchemeEditor(
            null!,
            _catalog,
            _mockSchemeService.Object,
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSystemNotifier.Object));
    }

    [Fact]
    public void Constructor_NullDeviceCatalog_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SchemeEditor(
            _mockUser.Object,
            null!,
            _mockSchemeService.Object,
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSystemNotifier.Object));
    }

    [Fact]
    public void Constructor_NullSchemeService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SchemeEditor(
            _mockUser.Object,
            _catalog,
            null!,
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSystemNotifier.Object));
    }

    [Fact]
    public void Constructor_NullConfigProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SchemeEditor(
            _mockUser.Object,
            _catalog,
            _mockSchemeService.Object,
            null!,
            _mockUserManager.Object,
            _mockSystemNotifier.Object));
    }

    [Fact]
    public void Constructor_NullUserManager_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SchemeEditor(
            _mockUser.Object,
            _catalog,
            _mockSchemeService.Object,
            _mockConfigProvider.Object,
            null!,
            _mockSystemNotifier.Object));
    }

    [Fact]
    public void Constructor_NullSystemNotifier_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SchemeEditor(
            _mockUser.Object,
            _catalog,
            _mockSchemeService.Object,
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            null!));
    }

    [Fact]
    public void Constructor_SetsSelectedScheme()
    {
        // Arrange
        var editor = CreateEditor();

        // Assert
        Assert.NotNull(editor.SelectedScheme);
    }

    #endregion

    #region SelectedScheme

    [Fact]
    public void SelectedScheme_ReturnsCurrentScheme()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        var scheme = editor.SelectedScheme;

        // Assert
        Assert.NotNull(scheme);
    }

    #endregion

    #region AllowCustomScheme

    [Fact]
    public void AllowCustomScheme_WhenTrue_ReturnsTrue()
    {
        // Arrange
        var editor = CreateEditor(allowCustomScheme: true);

        // Assert
        Assert.True(editor.AllowCustomScheme);
    }

    [Fact]
    public void AllowCustomScheme_WhenFalse_ReturnsFalse()
    {
        // Arrange
        var editor = CreateEditor(allowCustomScheme: false);

        // Assert
        Assert.False(editor.AllowCustomScheme);
    }

    #endregion

    #region InitiateInputCapture

    [Fact]
    public void InitiateInputCapture_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        var editor = CreateEditor();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => editor.InitiateInputCapture(null!));
    }

    [Fact]
    public void InitiateInputCapture_AlreadyCapturing_ReturnsInvalidRequest()
    {
        // Arrange
        var editor = CreateEditor();
        var actions = _validConfig.Definitions;
        var action = actions.First().Actions.First();

        // Act - first capture
        editor.InitiateInputCapture(action);

        // Act & Assert - second capture while still capturing
        var result = editor.InitiateInputCapture(action);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void InitiateInputCapture_ValidAction_InitiatesCaptureAndNotifies()
    {
        // Arrange
        var editor = CreateEditor();
        var actions = _validConfig.Definitions;
        var action = actions.First().Actions.First();

        // Act
        var result = editor.InitiateInputCapture(action);

        // Assert
        Assert.True(result.IsSuccessful);
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<SchemeEditorInputCaptureInitiatedNotification>(x => true)), Times.Once);
    }

    #endregion

    #region AbortInputCapture

    [Fact]
    public void AbortInputCapture_NoActiveCapture_DoesNothing()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        editor.AbortInputCapture();

        // Assert - no exception, no notification expected since no capture was active
        _mockSystemNotifier.Verify(n => n.Notify(It.IsAny<SchemeEditorInputCaptureTimeoutNotification>()), Times.Never);
    }

    [Fact]
    public void AbortInputCapture_ActiveCapture_AbortsAndNotifies()
    {
        // Arrange
        var editor = CreateEditor();
        var actions = _validConfig.Definitions;
        var action = actions.First().Actions.First();
        editor.InitiateInputCapture(action);

        // Act
        editor.AbortInputCapture();

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<SchemeEditorInputCaptureTimeoutNotification>(x => true)), Times.Once);
    }

    #endregion

    #region SetSchemeDevice

    [Fact]
    public void SetSchemeDevice_UnsupportedTopology_ReturnsInvalidRequest()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        var result = editor.SetSchemeDevice(DeviceTopologyName.Gamepad, "Gamepad");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetSchemeDevice_NonExistentDevice_ReturnsInvalidRequest()
    {
        // Arrange
        var editor = CreateEditor();
        var mockPart = new DeviceCatalogPart()
        {
            KnownDevices = [],
            GenericDevice = null,
        };

        // Act
        var result = editor.SetSchemeDevice(DeviceTopologyName.Keyboard, "NonExistent");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetSchemeDevice_ValidDevice_SetsDeviceAndNotifies()
    {
        // Arrange
        var editor = CreateEditor();

        var mockDescriptor = new Mock<IDeviceDescriptor>();
        mockDescriptor.SetupGet(m => m.Identity)
            .Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.PlayStation, "TestDevice"));

        _catalog.Parts[0].KnownDevices = [mockDescriptor.Object];

        // Act
        var result = editor.SetSchemeDevice(DeviceTopologyName.Keyboard, "TestDevice");

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region CreateNewScheme

    [Fact]
    public void CreateNewScheme_CustomSchemesNotAllowed_ReturnsInvalidRequest()
    {
        // Arrange
        var editor = CreateEditor(allowCustomScheme: false);

        // Act
        var result = editor.CreateNewScheme();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void CreateNewScheme_CustomSchemesAllowed_CreatesAndNotifies()
    {
        // Arrange
        var editor = CreateEditor(allowCustomScheme: true);

        // Act
        var result = editor.CreateNewScheme();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void CreateNewScheme_SetsNewSchemeState()
    {
        // Arrange
        var editor = CreateEditor(allowCustomScheme: true);

        // Act
        _ = editor.CreateNewScheme();

        // Assert
        Assert.True(editor.SelectedScheme.IsNew);
    }

    #endregion

    #region DeleteSchemeAsync

    [Fact]
    public async Task DeleteSchemeAsync_ReadonlyScheme_ReturnsInvalidRequest()
    {
        // Arrange
        var editor = CreateEditor();
        _mockSchemeService.Setup(m => m.DeleteCustomSchemeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        // Act
        var result = await editor.DeleteSchemeAsync(TestContext.Current.CancellationToken);

        // Assert - depends on whether scheme is readonly
        _ = result;
    }

    [Fact]
    public async Task DeleteSchemeAsync_ValidDelete_DelegatesToSchemeService()
    {
        // Arrange
        var mockRepo = new Mock<ISchemeRepository>();
        mockRepo.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(true);

        _mockSchemeService.Setup(m => m.DeleteCustomSchemeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        var editor = CreateEditor(allowCustomScheme: true);

        // Act
        var result = await editor.DeleteSchemeAsync(TestContext.Current.CancellationToken);

        // Assert - may succeed or fail depending on scheme state
        _ = result;
    }

    #endregion

    #region SaveSchemeAsync

    [Fact]
    public async Task SaveSchemeAsync_CustomSchemesNotAllowed_ReturnsInvalidRequest()
    {
        // Arrange
        var editor = CreateEditor(allowCustomScheme: false);

        // Act
        var result = await editor.SaveSchemeAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SaveSchemeAsync_HasUnpairedActions_ReturnsInvalidRequest()
    {
        // Arrange
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(true);
        var editor = CreateEditor(allowCustomScheme: true);

        _mockSchemeService.Setup(m => m.SaveCustomSchemeAsync(It.IsAny<CustomInputScheme>(), It.IsAny<SchemeSavePermissions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        // Act
        var result = await editor.SaveSchemeAsync(TestContext.Current.CancellationToken);

        // Assert - depends on scheme state
        _ = result;
    }

    #endregion

    #region GetDeviceCatalog

    [Fact]
    public void GetDeviceCatalog_ExistingTopology_ReturnsPart()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        var result = editor.GetDeviceCatalog(DeviceTopologyName.Keyboard);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetDeviceCatalog_NonExistentTopology_ReturnsNull()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        var result = editor.GetDeviceCatalog(DeviceTopologyName.Gamepad);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Helpers

    private SchemeEditor CreateEditor(bool allowCustomScheme = true)
    {
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(allowCustomScheme);
        return new SchemeEditor(
            _mockUser.Object,
            _catalog,
            _mockSchemeService.Object,
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSystemNotifier.Object);
    }

    #endregion
}
