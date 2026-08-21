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

    private readonly SchemeEditor _schemeEditor;

    #endregion

    #region Constructors

    public SchemeEditorTests()
    {
        _validConfig = TestConfigurationHelper.CreateValidConfiguration();
        _mockUser = new Mock<IInputUser>();
        _mockUser.SetupGet(m => m.Id).Returns(1);
        _mockSchemeService = new Mock<ISchemeService>();
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes)
            .Returns(false);
        
        _mockConfigProvider = new Mock<IInputSystemConfigurationProvider>();
        _mockConfigProvider.SetupGet(m => m.Configuration)
            .Returns(_validConfig);
        
        _mockUserManager = new Mock<IUserManager>();
        _mockSystemNotifier = new Mock<IInputSystemNotifier>();

        var page = new DevicePage(DeviceTopologyName.Keyboard, []);
        _catalog = new([page]);

        _schemeEditor = new SchemeEditor(_mockUser.Object, _catalog, _mockSchemeService.Object, _mockConfigProvider.Object,
            _mockUserManager.Object, _mockSystemNotifier.Object);
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
    public void Constructor_Valid_SetsPropertiesAsExpected()
    {
        // Assert
        Assert.NotNull(_schemeEditor.SelectedScheme);
    }

    #endregion

    #region SelectedScheme

    [Fact]
    public void SelectedScheme_ReturnsCurrentScheme()
    {
        // Arrange/Act
        var scheme = _schemeEditor.SelectedScheme;

        // Assert
        Assert.NotNull(scheme);
    }

    #endregion

    #region AllowCustomScheme

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AllowCustomScheme_ReturnsExpected(bool allow)
    {
        // Arrange
        _mockSchemeService.SetupGet(m => m.AllowCustomSchemes)
            .Returns(allow);

        // Assert
        Assert.Equal(allow, _schemeEditor.AllowCustomScheme);
    }

    #endregion

    #region InitiateInputCapture

    [Fact]
    public void InitiateInputCapture_NullAction_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _schemeEditor.InitiateInputCapture(null!));
    }

    [Fact]
    public void InitiateInputCapture_AlreadyCapturing_ReturnsInvalidRequest()
    {
        // Arrange
        var actions = _validConfig.Definitions;
        var action = actions.First().Actions.First();

        // Act - first capture
        _schemeEditor.InitiateInputCapture(action);

        // Act & Assert - second capture while still capturing
        var result = _schemeEditor.InitiateInputCapture(action);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void InitiateInputCapture_NonexistentAction_ReturnsInvalidRequest()
    {
        // Arrange
        var action = new InputAction("abc", new HashSet<InputPhase>(), _ => { });

        // Act - first capture
        _schemeEditor.InitiateInputCapture(action);

        // Act & Assert - second capture while still capturing
        var result = _schemeEditor.InitiateInputCapture(action);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void InitiateInputCapture_ValidAction_InitiatesCaptureAndNotifies()
    {
        // Arrange
        var actions = _validConfig.Definitions;
        var action = actions.First().Actions.First();

        // Act
        var result = _schemeEditor.InitiateInputCapture(action);

        // Assert
        Assert.True(result.IsSuccessful);
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<SchemeEditorInputCaptureInitiatedNotification>(x => true)), Times.Once);
    }

    #endregion

    #region AbortInputCapture

    [Fact]
    public void AbortInputCapture_NoActiveCapture_DoesNothing()
    {
        // Arrange/Act
        _schemeEditor.AbortInputCapture();

        // Assert - no exception, no notification expected since no capture was active
        _mockSystemNotifier.Verify(n => n.Notify(It.IsAny<SchemeEditorInputCaptureTimeoutNotification>()), Times.Never);
    }

    [Fact]
    public void AbortInputCapture_ActiveCapture_AbortsAndNotifies()
    {
        // Arrange
        var actions = _validConfig.Definitions;
        var action = actions.First().Actions.First();
        _schemeEditor.InitiateInputCapture(action);

        // Act
        _schemeEditor.AbortInputCapture();

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<SchemeEditorInputCaptureTimeoutNotification>(x => true)), Times.Once);
    }

    #endregion

    #region SetSchemeDevice

    [Fact]
    public void SetSchemeDevice_UnsupportedTopology_ReturnsInvalidRequest()
    {
        // Arrange/Act
        var result = _schemeEditor.SetSchemeDevice(DeviceTopologyName.Gamepad, "Gamepad");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetSchemeDevice_NonExistentDevice_ReturnsInvalidRequest()
    {
        // Arrange/Act
        var result = _schemeEditor.SetSchemeDevice(DeviceTopologyName.Keyboard, "NonExistent");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetSchemeDevice_ValidDevice_SetsDeviceAndNotifies()
    {
        // Arrange
        var mockDescriptor = new Mock<IDeviceDescriptor>();
        mockDescriptor.SetupGet(m => m.Identity)
            .Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.PlayStation, "TestDevice"));

        _catalog.Pages[0].Devices = [mockDescriptor.Object];

        // Act
        var result = _schemeEditor.SetSchemeDevice(DeviceTopologyName.Keyboard, "TestDevice");

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region CreateNewScheme

    [Fact]
    public void CreateNewScheme_CustomSchemesNotAllowed_ReturnsInvalidRequest()
    {
        // Arrange/Act
        var result = _schemeEditor.CreateNewScheme();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void CreateNewScheme_CustomSchemesAllowed_CreatesAndNotifies()
    {
        // Arrange
        _mockSchemeService.SetupGet(m => m.AllowCustomSchemes)
            .Returns(true);

        SchemeEditorUpdateTarget? target = null;
        _schemeEditor.EditorUpdated += t => target = t;

        // Act
        var result = _schemeEditor.CreateNewScheme();

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(_schemeEditor.SelectedScheme.IsNew);
        Assert.NotNull(target);
        Assert.Equal(SchemeEditorUpdateTarget.NewScheme, target);
    }

    #endregion

    #region DeleteSchemeAsync

    [Fact]
    public async Task DeleteSchemeAsync_ReadonlyScheme_ReturnsInvalidRequest()
    {
        // Arrange/Act
        var result = await _schemeEditor.DeleteSchemeAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);

        _mockSchemeService.Verify(m => m.DeleteCustomSchemeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteSchemeAsync_ValidDelete_DelegatesToSchemeService()
    {
        // Arrange
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes)
            .Returns(true);

        _schemeEditor.RefreshEditor(isNew: false, readOnlyOverride: false);

        _mockSchemeService.Setup(m => m.DeleteCustomSchemeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());


        // Act
        var result = await _schemeEditor.DeleteSchemeAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        _mockSchemeService.Verify(m => m.DeleteCustomSchemeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region SaveSchemeAsync

    [Fact]
    public async Task SaveSchemeAsync_CustomSchemesNotAllowed_ReturnsInvalidRequest()
    {
        // Arrange/Act
        var result = await _schemeEditor.SaveSchemeAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
        _mockSchemeService.Verify(m => m.SaveCustomSchemeAsync(It.IsAny<CustomInputScheme>(), It.IsAny<SchemeSavePermissions>()), Times.Never);
    }

    #endregion

    #region GetDevicePage

    [Fact]
    public void GetDevicePage_ExistingTopology_ReturnsPart()
    {
        // Arrange/Act
        var result = _schemeEditor.GetDevicePage(DeviceTopologyName.Keyboard);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DeviceTopologyName.Keyboard, result.TopologyName);
    }

    [Fact]
    public void GetDevicePage_NonExistentTopology_ReturnsNull()
    {
        // Arrange/Act
        var result = _schemeEditor.GetDevicePage(DeviceTopologyName.Gamepad);

        // Assert
        Assert.Null(result);
    }

    #endregion
}
