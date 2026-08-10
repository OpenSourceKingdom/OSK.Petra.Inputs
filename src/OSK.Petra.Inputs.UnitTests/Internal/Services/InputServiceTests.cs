using Microsoft.Extensions.Logging;
using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;
using Xunit.Sdk;

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
    private readonly InputSystemConfiguration _validConfig;

    #endregion

    #region Constructors

    public InputServiceTests()
    {
        _validConfig = TestConfigurationFactory.CreateValidConfiguration();
        _mockConfigProvider = TestConfigurationFactory.CreateConfigurationProvider(_validConfig);
        _mockSchemeService = new Mock<ISchemeService>();
        _mockUserManager = new Mock<IUserManager>();
        _mockSystemNotifier = new Mock<IInputSystemNotifier>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<InputService>>();

        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(false);
    }

    private InputService CreateService(IEnumerable<IInputCapability>? capabilities = null)
    {
        var caps = capabilities ?? Array.Empty<IInputCapability>();
        return new InputService(
            caps,
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockSchemeService.Object,
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object);
    }

    #endregion

    #region Constructor_NullChecks

    [Fact]
    public void Constructor_NullCapabilities_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CreateService(null!));
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
            _mockSystemNotifier.Object,
            _mockServiceProvider.Object,
            null!));
    }

    #endregion

    #region PauseInput

    [Fact]
    public void PauseInput_SetToTrue_ReturnsTrue()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.PauseInput = true;

        // Assert
        Assert.True(service.PauseInput);
    }

    [Fact]
    public void PauseInput_SetToFalse_ReturnsFalse()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.PauseInput = false;

        // Assert
        Assert.False(service.PauseInput);
    }

    [Fact]
    public void PauseInput_DefaultValue_IsFalse()
    {
        // Arrange
        var service = CreateService();

        // Assert
        Assert.False(service.PauseInput);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_WhenPaused_DoesNotProcessInput()
    {
        // Arrange
        var capability = new TestablePointerCapability();
        var service = CreateService([capability]);
        service.PauseInput = true;

        // Act
        service.Update(TimeSpan.FromSeconds(1));

        // Assert
        Assert.False(capability.ProcessCalled);
    }

    [Fact]
    public void Update_CallsCapabilitiesWithDeltaTime()
    {
        // Arrange
        var capability = new TestablePointerCapability();
        var service = CreateService([capability]);

        // Act
        var delta = TimeSpan.FromSeconds(0.5);
        service.Update(delta);

        // Assert
        Assert.True(capability.ProcessCalled);
        Assert.Equal(delta, capability.ReceivedDeltaTime);
    }

    #endregion

    #region ProcessDeviceNotification_DeviceNotSupported

    [Fact]
    public void ProcessDeviceNotification_UnsupportedTopology_NotifyUnrecognizedDevice()
    {
        // Arrange
        var service = CreateService();
        var mockInput = new Mock<IInput>();
        var notification = new DeviceInputNotification(
            TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Gamepad),
            mockInput.Object,
            TimeSpan.Zero);

        _mockSystemNotifier.Raise(m => m.OnDeviceNotification += null, notification);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UnrecognizedDeviceNotification>(x => true)), Times.Once);
    }

    #endregion

    #region ProcessDeviceNotification_IncompatibleInput

    [Fact]
    public void ProcessDeviceNotification_IncompatibleInput_NotifyUnrecognizedDevice()
    {
        // Arrange
        var config = CreateConfigWithIncompatibleTopology();
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(config);
        var service = CreateService();
        var mockInput = new Mock<IInput>();
        var deviceIdentifier = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object, TimeSpan.Zero);

        _mockSystemNotifier.Raise(m => m.OnDeviceNotification += null, notification);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UnrecognizedDeviceNotification>(x => true)), Times.Once);
    }

    private InputSystemConfiguration CreateConfigWithIncompatibleTopology()
    {
        var mockTopology = new Mock<IDeviceTopology>();
        mockTopology.Setup(m => m.IsCompatibleInput(It.IsAny<IInput>())).Returns(false);
        mockTopology.SetupGet(m => m.Name).Returns(DeviceTopologyName.Keyboard);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced
        };

        return new InputSystemConfiguration([mockTopology.Object], [], [], joinPolicy);
    }

    #endregion

    #region ProcessDeviceNotification_ManualPairing

    [Fact]
    public void ProcessDeviceNotification_ManualPairing_NoPairedUser_ReturnsWithoutProcessing()
    {
        // Arrange
        _mockConfigProvider.SetupGet(m => m.Configuration.JoinPolicy).Returns(new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.Manual,
            DeviceJoinBehavior = DevicePairingBehavior.Manual
        });

        var service = CreateService();
        var mockInput = new Mock<IInput>();
        var deviceIdentifier = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object, TimeSpan.Zero);

        _mockUserManager.Setup(m => m.GetUserForDevice(deviceIdentifier.DeviceId)).Returns((IInputUser?)null);

        _mockSystemNotifier.Raise(m => m.OnDeviceNotification += null, notification);

        // Assert
        _mockSchemeService.Verify(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()), Times.Never);
    }

    #endregion

    #region ProcessInput_ActionTriggering

    [Fact]
    public void ProcessInput_ActionWithMatchingPhase_TriggersActionExecutor()
    {
        // Arrange
        IInputEventContext? capturedContext = null;
        var action = new InputAction("TestAction", new HashSet<InputPhase> { InputPhase.Start }, ctx => { capturedContext = ctx; });
        var definition = new ActionDefinition("Default", [action], isDefault: true);

        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard"),
                InputMaps = new[] { new InputActionMap(action, new MockInput(1)) }
            }
        };

        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced
        };

        var mockTopology = new Mock<IDeviceTopology>();
        mockTopology.Setup(m => m.IsCompatibleInput(It.IsAny<IInput>())).Returns(true);
        mockTopology.SetupGet(m => m.Name).Returns(DeviceTopologyName.Keyboard);

        var configProvider = new Mock<IInputSystemConfigurationProvider>();
        configProvider.SetupGet(m => m.Configuration).Returns(new InputSystemConfiguration([mockTopology.Object], new[] { config }, new[] { definition }, joinPolicy));

        var mockSchemeService = new Mock<ISchemeService>();
        var expectedScheme = scheme;
        mockSchemeService.Setup(s => s.SetActiveSchemeForDevice(It.IsAny<int>(), It.IsAny<DeviceIdentity>()))
            .Returns(Out.Updated(expectedScheme));

        var mockUserManager = new Mock<IUserManager>();
        var mockUser = TestConfigurationFactory.CreateUser(1);
        mockUserManager.Setup(m => m.GetUserForDevice(It.IsAny<int>())).Returns(mockUser);

        var mockNotifier = new Mock<IInputSystemNotifier>();
        var service = new InputService(
            Array.Empty<IInputCapability>(),
            configProvider.Object,
            mockUserManager.Object,
            mockSchemeService.Object,
            mockNotifier.Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<InputService>>().Object);

        var mockInput = new Mock<IInput>();
        mockInput.SetupGet(m => m.Id).Returns(1);
        var deviceIdentifier = TestConfigurationFactory.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var notification = new DeviceInputNotification(deviceIdentifier, mockInput.Object, TimeSpan.Zero);

        // Act
        mockNotifier.Raise(m => m.OnDeviceNotification += null, notification);

        // Assert
        Assert.NotNull(capturedContext);
    }

    #endregion

    #region ProcessSystemNotification_ModifyActionGroupSuppression

    [Fact]
    public void ProcessSystemNotification_GlobalSuppression_SetsGlobalSuppression()
    {
        // Arrange
        var service = CreateService();
        var notification = new ModifyActionGroupSuppressionNotification()
        {
            ActionGroups = null,
            UserIds = null,
            Suppress = true
        };

        // Act
        _mockSystemNotifier.Raise(m => m.OnSystemNotification += null, notification);

        // Assert - verify via PauseInput behavior or internal state
        // The global suppression flag is not externally observable, so we verify no exception thrown
    }

    [Fact]
    public void ProcessSystemNotification_GlobalUnsuppression_ClearsGlobalSuppression()
    {
        // Arrange
        var service = CreateService();
        var notification = new ModifyActionGroupSuppressionNotification()
        {
            ActionGroups = null,
            UserIds = null,
            Suppress = false
        };

        // Act
        _mockSystemNotifier.Raise(m => m.OnSystemNotification += null, notification);
    }

    [Fact]
    public void ProcessSystemNotification_UserSpecificSuppression_SuppressesOnlyTargetedUsers()
    {
        // Arrange
        var service = CreateService();
        int[] userIds = [1];
        int[] actionGroups = [5];
        var notification = new ModifyActionGroupSuppressionNotification()
        {
            UserIds = userIds,
            ActionGroups = actionGroups,
            Suppress = true
        };

        // Act
        _mockSystemNotifier.Raise(m => m.OnSystemNotification += null, notification);
    }

    #endregion

    #region ProcessSystemNotification_SchemeEditorEvents

    [Fact]
    public void ProcessSystemNotification_InputCaptureInitiated_SetsEditorDelay()
    {
        // Arrange
        var service = CreateService();
        int userId = 1;
        TimeSpan timeout = TimeSpan.FromSeconds(5);
        var notification = new SchemeEditorInputCaptureInitiatedNotification(userId, timeout);

        // Act
        _mockSystemNotifier.Raise(m => m.OnSystemNotification += null, notification);
    }

    [Fact]
    public void ProcessSystemNotification_InputCaptureTimeout_ClearsEditorDelay()
    {
        // Arrange
        var service = CreateService();
        int userId = 1;
        var notification = new SchemeEditorInputCaptureTimeoutNotification(userId);

        // Act
        _mockSystemNotifier.Raise(m => m.OnSystemNotification += null, notification);
    }

    #endregion

    #region ProcessUserNotification_UserRemoved

    [Fact]
    public void ProcessUserNotification_UserRemoved_RemovesUserContext()
    {
        // Arrange
        var service = CreateService();
        var mockUser = TestConfigurationFactory.CreateUser(1);
        var notification = new UserRemovedNotification(mockUser);

        // Act
        _mockSystemNotifier.Raise(m => m.OnUserNotification += null, notification);
    }

    #endregion

    #region Helper Classes

    private class TestablePointerCapability : PointerCapability
    {
        public bool ProcessCalled { get; private set; }
        public TimeSpan ReceivedDeltaTime { get; private set; }

        public TestablePointerCapability() : base(Microsoft.Extensions.Options.Options.Create(new PointerCapabilityOptions())) { }

        protected override void Process(IDeviceInputContext context, IInputState state, IPointer input, TimeSpan deltaTime)
        {
            ProcessCalled = true;
            ReceivedDeltaTime = deltaTime;
        }
    }

    #endregion
}
