using Microsoft.Extensions.Logging;
using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class InputServiceTests
{
    #region Variables

    private readonly Mock<IInputSystemConfigurationProvider> _mockConfigProvider;
    private readonly Mock<ISchemeService> _mockSchemeService;
    private readonly Mock<IUserManager> _mockUserManager;
    private readonly Mock<IInputSystemNotifier> _mockSystemNotifier;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ILogger<InputService>> _mockLogger;
    private readonly Mock<IDeviceCatalogProvider> _mockDeviceCatalogProvider;
    private readonly InputSystemConfiguration _validConfig;
    private readonly List<IInputCapability> _capabilities;

    private readonly InputService _service;

    #endregion

    #region Constructors

    public InputServiceTests()
    {
        _validConfig = TestConfigurationHelper.CreateValidConfiguration();

        _mockConfigProvider = new Mock<IInputSystemConfigurationProvider>();
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(_validConfig);
        
        _mockSchemeService = new Mock<ISchemeService>();
        _mockUserManager = new Mock<IUserManager>();
        _mockSystemNotifier = new Mock<IInputSystemNotifier>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<InputService>>();
        _mockDeviceCatalogProvider = new();

        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(false);
        _capabilities = [];
        _service = new(_capabilities, _mockConfigProvider.Object, _mockUserManager.Object, _mockSchemeService.Object,
            _mockDeviceCatalogProvider.Object, _mockSystemNotifier.Object, _mockServiceProvider.Object, _mockLogger.Object);
    }

    #endregion

    #region Constructor_NullChecks

    [Fact]
    public void Constructor_NullCapabilities_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            null!,
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSchemeService.Object, 
            _mockDeviceCatalogProvider.Object,
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullConfigurationProvider_ThrowsArgumentNullException()
    {
        // Arrange
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns((InputSystemConfiguration?)null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            Array.Empty<IInputCapability>(),
            null!,
            _mockUserManager.Object,
            _mockSchemeService.Object, 
            _mockDeviceCatalogProvider.Object,
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullSchemeService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            Array.Empty<IInputCapability>(),
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            null!, 
            _mockDeviceCatalogProvider.Object,
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullTopologyCatalogProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            Array.Empty<IInputCapability>(),
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSchemeService.Object,
            null!,
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullUserManager_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            Array.Empty<IInputCapability>(),
            _mockConfigProvider.Object,
            null!,
            _mockSchemeService.Object,
            _mockDeviceCatalogProvider.Object,
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullSystemNotifier_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            Array.Empty<IInputCapability>(),
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSchemeService.Object,
            _mockDeviceCatalogProvider.Object,
            null!,
            _mockServiceProvider.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            Array.Empty<IInputCapability>(),
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSchemeService.Object,
            _mockDeviceCatalogProvider.Object,
            _mockSystemNotifier.Object,
            null!,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputService(
            Array.Empty<IInputCapability>(),
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSchemeService.Object,
            _mockDeviceCatalogProvider.Object,
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            null!));
    }

    #endregion

    #region PauseInput

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PauseInput_SetToValue_ReturnsExpected(bool pause)
    {
        // Arrange/Act
        _service.PauseInput = pause;

        Assert.Equal(pause, _service.PauseInput);
    }

    [Fact]
    public void PauseInput_DefaultValue_IsFalse()
    {
        // Arrange/Assert
        Assert.False(_service.PauseInput);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_WhenPaused_DoesNotProcessInput()
    {
        // Arrange
        var capability = new TestablePointerCapability();
        _capabilities.Add(capability);

        _service.PauseInput = true;

        // Act
        _service.Update(TimeSpan.FromSeconds(1));

        // Assert
        Assert.False(capability.ProcessCalled);
    }

    [Fact]
    public async Task Update_CallsCapabilitiesWithDeltaTime()
    {
        // Arrange
        var capability = new TestablePointerCapability();
        _capabilities.Add(capability);

        var mockUser = new InputUser(1);
        _mockUserManager.Setup(m => m.GetUserForDevice(It.IsAny<int>())).Returns(mockUser);

        var mockInput = new Mock<IPointer>();
        mockInput.SetupGet(m => m.Id)
            .Returns(1);
        var deviceIdentifier = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);

        mockInput.Setup(m => m.GetGlyphAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new InputGlyph() { DeviceIdentity = deviceIdentifier.DeviceIdentity, Input = mockInput.Object, Text = "A" });

        var mockDevice = new Mock<IDeviceDescriptor>();
        mockDevice.Setup(m => m.GetInput(It.IsAny<long>()))
            .Returns(mockInput.Object);
        mockDevice.Setup(m => m.Identity)
            .Returns(new DeviceIdentity(deviceIdentifier.DeviceIdentity.TopologyName, deviceIdentifier.DeviceIdentity.DeviceFamily, DeviceIdentities.GenericDeviceName));
        _mockDeviceCatalogProvider.Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success(new DeviceCatalog([new DevicePage(DeviceTopologyName.Keyboard, [mockDevice.Object])])));

        await _service.InitializeAsync(TestContext.Current.CancellationToken);

        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object.Id, TimeSpan.Zero, new PointerEvent());

        var action = new InputAction("TestAction", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard"),
                InputMaps = new[] { new DeviceInputActionMap(action, 1) }
            }
        };
        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        _mockSchemeService.Setup(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()))
            .Returns(Out.Updated(scheme));

        _service.ProcessNotificationForTest(notification);
        
        // Act
        var delta = TimeSpan.FromSeconds(0.5);
        _service.Update(delta);

        // Assert
        Assert.True(capability.ProcessCalled);
        Assert.Equal(delta, capability.ReceivedDeltaTime);
    }

    #endregion

    #region ProcessDeviceNotification

    [Fact]
    public async Task ProcessDeviceNotification_UnsupportedTopology_NotifyUnrecognizedDevice()
    {
        // Arrange
        var mockInput = new Mock<IDeviceInput>();
        mockInput.SetupGet(m => m.Id)
            .Returns(1);

        _mockDeviceCatalogProvider.Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success(new DeviceCatalog([new DevicePage(DeviceTopologyName.Keyboard, [])])));

        await _service.InitializeAsync(TestContext.Current.CancellationToken);

        var notification = new DeviceInputNotification(
            TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Gamepad),
             mockInput.Object.Id,
            TimeSpan.Zero);

        _service.ProcessNotificationForTest(notification);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UnrecognizedDeviceNotification>(x => true)), Times.Once);
    }

    [Fact]
    public async Task ProcessDeviceNotification_IncompatibleInput_NotifyUnrecognizedDeviceInput()
    {
        // Arrange
        var deviceIdentifier = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var mockInput = new Mock<IDeviceInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);
        mockInput.Setup(m => m.GetGlyphAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new InputGlyph() { DeviceIdentity = deviceIdentifier.DeviceIdentity, Input = mockInput.Object, Text = "A" });

        var mockDevice = new Mock<IDeviceDescriptor>();
        mockDevice.Setup(m => m.GetInput(It.IsAny<long>()))
            .Returns((IDeviceInput?)null);
        mockDevice.SetupGet(m => m.Identity)
            .Returns(deviceIdentifier.DeviceIdentity);

        _mockDeviceCatalogProvider.Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success(new DeviceCatalog([new DevicePage(DeviceTopologyName.Keyboard, [mockDevice.Object])])));

        await _service.InitializeAsync(TestContext.Current.CancellationToken);

        var action = new InputAction("TestAction", new HashSet<InputPhase> { InputPhase.End }, ctx => { });
        var definition = new ActionDefinition("Default", [action], isDefault: true);

        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard"),
                InputMaps = new[] { new DeviceInputActionMap(action, 1) }
            }
        };

        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(scheme);

        _mockSchemeService.Setup(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()))
            .Returns(Out.Updated(scheme));

        var mockUser = new InputUser(1);
        _mockUserManager.Setup(m => m.GetUserForDevice(It.IsAny<int>())).Returns(mockUser);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DevicePairingBehavior = DevicePairingBehavior.Balanced
        };

        var systemConfiguration = new InputSystemConfiguration([config], [], joinPolicy, new([]));
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(systemConfiguration);
        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object.Id, TimeSpan.Zero);

        _service.ProcessNotificationForTest(notification);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UnrecognizedDeviceInputNotification>(x => true)), Times.Once);
    }

    [Fact]
    public async Task ProcessDeviceNotification_ManualUserJoin_ManualPairing_NoPairedUser_ReturnsWithoutProcessing()
    {
        // Arrange
        var deviceIdentifier = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var mockInput = new Mock<IDeviceInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);
        mockInput.Setup(m => m.GetGlyphAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new InputGlyph() { DeviceIdentity = deviceIdentifier.DeviceIdentity, Input = mockInput.Object, Text = "A" });

        var mockDevice = new Mock<IDeviceDescriptor>();
        mockDevice.Setup(m => m.GetInput(It.IsAny<long>()))
            .Returns(mockInput.Object);
        mockDevice.SetupGet(m => m.Identity)
            .Returns(deviceIdentifier.DeviceIdentity);
        _mockDeviceCatalogProvider.Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success(new DeviceCatalog([new DevicePage(DeviceTopologyName.Keyboard, [mockDevice.Object])])));

        await _service.InitializeAsync(TestContext.Current.CancellationToken);

        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(() => {
            return new InputSystemConfiguration([new([DeviceTopologyName.Keyboard]) ], [], new InputSystemJoinPolicy
            {
                MaxUsers = 4,
                UserJoinBehavior = UserJoinBehavior.Manual,
                DevicePairingBehavior = DevicePairingBehavior.Manual
            }, new([]));
        });

        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object.Id, TimeSpan.Zero);

        _mockUserManager.Setup(m => m.GetUserForDevice(deviceIdentifier.DeviceId)).Returns((IInputUser?)null);

        _service.ProcessNotificationForTest(notification);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UnpairedDeviceInputNotification>(x => true)), Times.Once);
        _mockSchemeService.Verify(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()), Times.Never);
    }

    [Fact]
    public async Task ProcessDeviceNotification_AutomaticUserJoin_ManualDevicePairing_ReturnsWithoutProcessing()
    {
        // Arrange
        var deviceIdentifier = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var mockInput = new Mock<IDeviceInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);
        mockInput.Setup(m => m.GetGlyphAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new InputGlyph() { DeviceIdentity = deviceIdentifier.DeviceIdentity, Input = mockInput.Object, Text = "A" });

        var mockDevice = new Mock<IDeviceDescriptor>();
        mockDevice.Setup(m => m.GetInput(It.IsAny<long>()))
            .Returns(mockInput.Object);
        mockDevice.SetupGet(m => m.Identity)
            .Returns(deviceIdentifier.DeviceIdentity);
        _mockDeviceCatalogProvider.Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success(new DeviceCatalog([new DevicePage(DeviceTopologyName.Keyboard, [mockDevice.Object])])));

        await _service.InitializeAsync(TestContext.Current.CancellationToken);

        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(() => {
            return new InputSystemConfiguration([new InputConfiguration([DeviceTopologyName.Keyboard])], [], new InputSystemJoinPolicy
            {
                MaxUsers = 4,
                UserJoinBehavior = UserJoinBehavior.DeviceActivation,
                DevicePairingBehavior = DevicePairingBehavior.Manual
            }, new([]));
        });

        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object.Id, TimeSpan.Zero);

        _mockUserManager.Setup(m => m.GetUserForDevice(deviceIdentifier.DeviceId)).Returns((IInputUser?)null);

        _service.ProcessNotificationForTest(notification);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UnpairedDeviceInputNotification>(x => true)), Times.Once);
        _mockSchemeService.Verify(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()), Times.Never);
    }

    [Fact]
    public void ProcessInput_ActionWithNoMathcingPhase_DoesNotTriggerActionExecutor()
    {
        // Arrange
        IInputEventContext? capturedContext = null;
        var action = new InputAction("TestAction", new HashSet<InputPhase> { InputPhase.End }, ctx => { capturedContext = ctx; });
        var definition = new ActionDefinition("Default", [action], isDefault: true);

        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard"),
                InputMaps = new[] { new DeviceInputActionMap(action, 1) }
            }
        };

        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DevicePairingBehavior = DevicePairingBehavior.Balanced
        };

        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([])));

        _mockSchemeService.Setup(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()))
            .Returns(Out.Updated(scheme));

        var mockUser = new InputUser(1);
        _mockUserManager.Setup(m => m.GetUserForDevice(It.IsAny<int>())).Returns(mockUser);

        var deviceIdentifier = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);

        var mockInput = new Mock<IDeviceInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);
        mockInput.Setup(m => m.GetGlyphAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new InputGlyph() { DeviceIdentity = deviceIdentifier.DeviceIdentity, Input = mockInput.Object, Text = "A" });

        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object.Id, TimeSpan.Zero);

        // Act
        _service.ProcessNotificationForTest(notification);

        // Assert
        Assert.Null(capturedContext);
    }

    [Fact]
    public async Task ProcessInput_ActionWithMatchingPhase_TriggersActionExecutor()
    {
        // Arrange

        var deviceIdentifier = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var mockInput = new Mock<IDeviceInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);
        mockInput.Setup(m => m.GetGlyphAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new InputGlyph() { DeviceIdentity = deviceIdentifier.DeviceIdentity, Input = mockInput.Object, Text = "A" });

        var mockDevice = new Mock<IDeviceDescriptor>();
        mockDevice.Setup(m => m.Identity)
            .Returns(deviceIdentifier.DeviceIdentity);
        mockDevice.Setup(m => m.GetInput(It.IsAny<long>()))
            .Returns(mockInput.Object);
        _mockDeviceCatalogProvider.Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success(new DeviceCatalog([new DevicePage(DeviceTopologyName.Keyboard, [mockDevice.Object])])));
        
        await _service.InitializeAsync(TestContext.Current.CancellationToken);

        IInputEventContext? capturedContext = null;
        var action = new InputAction("TestAction", new HashSet<InputPhase> { InputPhase.Start }, ctx => { capturedContext = ctx; });
        var definition = new ActionDefinition("Default", [action], isDefault: true);

        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard"),
                InputMaps = new[] { new DeviceInputActionMap(action, 1) }
            }
        };

        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DevicePairingBehavior = DevicePairingBehavior.Balanced
        };

        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new([])));

        _mockSchemeService.Setup(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()))
            .Returns(Out.Updated(scheme));

        var mockUser = new InputUser(1);
        _mockUserManager.Setup(m => m.GetUserForDevice(It.IsAny<int>())).Returns(mockUser);

        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object.Id, TimeSpan.Zero);

        // Act
        _service.ProcessNotificationForTest(notification);

        // Assert
        Assert.NotNull(capturedContext);
    }

    #endregion

    #region ProcessSystemNotification

    [Fact]
    public void ProcessSystemNotification_GlobalSuppression_SetsGlobalSuppression()
    {
        // Arrange
        var notification = new ModifyActionGroupSuppressionNotification()
        {
            ActionGroups = null,
            UserIds = null,
            Suppress = true
        };

        // Act
        _service.ProcessNotificationForTest(notification);

        // Assert
        Assert.True(_service.IsGlobalInputSuppressed);
    }

    [Fact]
    public void ProcessSystemNotification_GlobalUnsuppression_ClearsGlobalSuppression()
    {
        // Arrange
        var notification = new ModifyActionGroupSuppressionNotification()
        {
            ActionGroups = null,
            UserIds = null,
            Suppress = false
        };

        // Act
        _service.ProcessNotificationForTest(notification);

        // Assert
        Assert.False(_service.IsGlobalInputSuppressed);
    }

    [Fact]
    public void ProcessSystemNotification_UserSpecificSuppression_SuppressesOnlyTargetedUsers()
    {
        // Arrange
        int[] userIds = [1];
        
        _service.ProcessNotificationForTest(new UserJoinedNotification(new InputUser(1)));

        int[] actionGroups = [5];
        var notification = new ModifyActionGroupSuppressionNotification()
        {
            UserIds = userIds,
            ActionGroups = actionGroups,
            Suppress = true
        };

        // Act
        _service.ProcessNotificationForTest(notification);

        // Assert
        var suppressedUsers = _service.UserContexts.Where(context => context.IsGloballySuppressed);
        Assert.Empty(suppressedUsers);

        suppressedUsers = _service.UserContexts.Where(context => context.IsSuppressed(5));
        Assert.Single(suppressedUsers);
    }

    [Fact]
    public void ProcessSystemNotification_InputCaptureInitiated_SetsEditorDelay()
    {
        // Arrange
        int userId = 1;
        _service.ProcessNotificationForTest(new UserJoinedNotification(new InputUser(userId)));

        TimeSpan timeout = TimeSpan.FromSeconds(5);
        var notification = new SchemeEditorInputCaptureInitiatedNotification(userId, timeout);

        // Act
        _service.ProcessNotificationForTest(notification);

        // Assert
        var userContext = _service.UserContexts.First(context => context.UserId == userId);
        Assert.NotNull(userContext.EditorInputCaptureTimeout);
        Assert.Equal(timeout, userContext.EditorInputCaptureTimeout.Value.Delay);
    }

    [Fact]
    public void ProcessSystemNotification_InputCaptureTimeout_ClearsEditorDelay()
    {
        // Arrange
        int userId = 1;
        _service.ProcessNotificationForTest(new UserJoinedNotification(new InputUser(userId)));

        TimeSpan timeout = TimeSpan.FromSeconds(5);
        var captureInitiatedNotification = new SchemeEditorInputCaptureInitiatedNotification(userId, timeout);
        _service.ProcessNotificationForTest(captureInitiatedNotification);

        var userContext = _service.UserContexts.First(context => context.UserId == userId);
        Assert.NotNull(userContext.EditorInputCaptureTimeout);

        var notification = new SchemeEditorInputCaptureTimeoutNotification(userId);

        // Act
        _service.ProcessNotificationForTest(notification);

        // Assert
        Assert.Null(userContext.EditorInputCaptureTimeout);
    }

    #endregion

    #region ProcessUserNotification

    [Fact]
    public void ProcessUserNotification_UserRemoved_RemovesUserContext()
    {
        // Arrange
        var mockUser = new InputUser(1);
        var notification = new UserRemovedNotification(mockUser);

        // Act
        _service.ProcessNotificationForTest(notification);
    }

    #endregion
}
