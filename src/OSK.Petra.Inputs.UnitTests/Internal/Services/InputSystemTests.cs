using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class InputSystemTests
{
    #region Variables

    private readonly Mock<IInputSystemConfigurationProvider> _mockConfigProvider;
    private readonly Mock<IUserManager> _mockUserManager;
    private readonly Mock<IInputService> _mockInputService;
    private readonly Mock<IInputSystemNotifier> _mockSystemNotifier;
    private readonly Mock<IInternalSchemeService> _mockSchemeService;
    private readonly InputSystemConfiguration _validConfig;

    #endregion

    #region Constructors

    public InputSystemTests()
    {
        _validConfig = TestConfigurationFactory.CreateValidConfiguration();
        _mockConfigProvider = TestConfigurationFactory.CreateConfigurationProvider(_validConfig);
        _mockUserManager = new Mock<IUserManager>();
        _mockInputService = new Mock<IInputService>();
        _mockSystemNotifier = new Mock<IInputSystemNotifier>();
        _mockSchemeService = new Mock<IInternalSchemeService>();
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(false);
    }

    private InputSystem CreateSystem()
    {
        return new InputSystem(
            _mockConfigProvider.Object,
            _mockUserManager.Object,
            _mockInputService.Object,
            _mockSystemNotifier.Object,
            _mockSchemeService.Object);
    }

    #endregion

    #region Configuration

    [Fact]
    public void Configuration_ReturnsConfigurationProviderValue()
    {
        // Arrange
        var system = CreateSystem();

        // Act
        var config = system.Configuration;

        // Assert
        Assert.NotNull(config);
        Assert.Same(_validConfig, config);
    }

    #endregion

    #region Notifier

    [Fact]
    public void Notifier_ReturnsSystemNotifier()
    {
        // Arrange
        var system = CreateSystem();

        // Act
        var notifier = system.Notifier;

        // Assert
        Assert.NotNull(notifier);
        Assert.Same(_mockSystemNotifier.Object, notifier);
    }

    #endregion

    #region UserManager

    [Fact]
    public void UserManager_ReturnsUserManager()
    {
        // Arrange
        var system = CreateSystem();

        // Act
        var userManager = system.UserManager;

        // Assert
        Assert.NotNull(userManager);
        Assert.Same(_mockUserManager.Object, userManager);
    }

    #endregion

    #region SchemeService

    [Fact]
    public void SchemeService_ReturnsSchemeService()
    {
        // Arrange
        var system = CreateSystem();

        // Act
        var schemeService = system.SchemeService;

        // Assert
        Assert.NotNull(schemeService);
        Assert.Same(_mockSchemeService.Object, schemeService);
    }

    [Fact]
    public void AllowCustomSchemes_ReturnsSchemeServiceValue()
    {
        // Arrange
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(true);
        var system = CreateSystem();

        // Act
        Assert.True(system.AllowCustomSchemes);
    }

    #endregion

    #region PauseInput

    [Fact]
    public void PauseInput_GetDefault_ReturnsFalse()
    {
        // Arrange
        var system = CreateSystem();

        // Assert
        Assert.False(system.PauseInput);
    }

    [Fact]
    public void PauseInput_SetToTrue_SetsInternalAndDelegates()
    {
        // Arrange
        var system = CreateSystem();

        // Act
        system.PauseInput = true;

        // Assert
        Assert.True(system.PauseInput);
        _mockInputService.VerifySet(s => s.PauseInput = true, Times.Once);
    }

    [Fact]
    public void PauseInput_SetToFalse_SetsInternalAndDelegates()
    {
        // Arrange
        var system = CreateSystem();
        system.PauseInput = true;

        // Act
        system.PauseInput = false;

        // Assert
        Assert.False(system.PauseInput);
        _mockInputService.VerifySet(s => s.PauseInput = false, Times.Once);
    }

    [Fact]
    public void PauseInput_SetSameValue_DoesNotNotify()
    {
        // Arrange
        var system = CreateSystem();

        // Act - set to true twice
        system.PauseInput = true;
        _mockSystemNotifier.Invocations.Clear();
        system.PauseInput = true;

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.IsAny<InputMonitorStatusChangedNotification>()), Times.Never);
    }

    [Fact]
    public void PauseInput_SetTrue_RaisesMonitorStatusChangedNotification()
    {
        // Arrange
        var system = CreateSystem();

        // Act
        system.PauseInput = true;

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<InputMonitorStatusChangedNotification>(x => !x.IsMonitoringInput)), Times.Once);
    }

    [Fact]
    public void PauseInput_SetFalse_RaisesMonitorStatusChangedNotification()
    {
        // Arrange
        var system = CreateSystem();
        system.PauseInput = true;
        _mockSystemNotifier.Invocations.Clear();

        // Act
        system.PauseInput = false;

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<InputMonitorStatusChangedNotification>(x => x.IsMonitoringInput)), Times.Once);
    }

    #endregion

    #region InitializeAsync

    [Fact]
    public async Task InitializeAsync_ValidConfiguration_ReturnsSuccess()
    {
        // Arrange
        var system = CreateSystem();
        _mockSchemeService.Setup(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success()));

        // Act
        var result = await system.InitializeAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void InitializeAsync_InvalidConfiguration_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidConfig = CreateInvalidConfiguration();
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(invalidConfig);
        var system = CreateSystem();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => system.InitializeAsync().GetAwaiter().GetResult());
    }

    [Fact]
    public void InitializeAsync_CallsSchemeServiceLoad()
    {
        // Arrange
        var system = CreateSystem();
        _mockSchemeService.Setup(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success()));

        // Act
        _ = system.InitializeAsync();

        // Assert
        _mockSchemeService.Verify(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private InputSystemConfiguration CreateInvalidConfiguration()
    {
        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 0,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced
        };
        return new InputSystemConfiguration([], [], [], joinPolicy);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_WhenPaused_DoesNotDelegateToInputService()
    {
        // Arrange
        var system = CreateSystem();
        system.PauseInput = true;

        // Act
        system.Update(TimeSpan.FromSeconds(1));

        // Assert
        _mockInputService.Verify(s => s.Update(It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public void Update_WhenNotPaused_DelegatesToInputService()
    {
        // Arrange
        var system = CreateSystem();
        system.PauseInput = false;
        var delta = TimeSpan.FromSeconds(0.1);

        // Act
        system.Update(delta);

        // Assert
        _mockInputService.Verify(s => s.Update(delta), Times.Once);
    }

    [Fact]
    public void Update_PassesCorrectDeltaTime()
    {
        // Arrange
        var system = CreateSystem();
        var delta = TimeSpan.FromMilliseconds(16);

        // Act
        system.Update(delta);

        // Assert
        _mockInputService.Verify(s => s.Update(delta), Times.Once);
    }

    #endregion
}
