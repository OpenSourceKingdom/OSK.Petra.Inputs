using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
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

    private readonly InputSystem _inputSystem;

    #endregion

    #region Constructors

    public InputSystemTests()
    {
        _validConfig = TestConfigurationHelper.CreateValidConfiguration();
        
        _mockConfigProvider = new Mock<IInputSystemConfigurationProvider>();
        _mockConfigProvider.SetupGet(m => m.Configuration)
            .Returns(_validConfig);
       
        _mockUserManager = new Mock<IUserManager>();
        _mockInputService = new Mock<IInputService>();
        _mockSystemNotifier = new Mock<IInputSystemNotifier>();
        _mockSchemeService = new Mock<IInternalSchemeService>();
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(false);

        _inputSystem = new(_mockConfigProvider.Object, _mockUserManager.Object, _mockInputService.Object, _mockSystemNotifier.Object, _mockSchemeService.Object);
    }

    #endregion

    #region Configuration

    [Fact]
    public void Configuration_ReturnsConfigurationProviderValue()
    {
        // Arrange/Act
        var config = _inputSystem.Configuration;

        // Assert
        Assert.NotNull(config);
        Assert.Same(_validConfig, config);
    }

    #endregion

    #region Notifier

    [Fact]
    public void Notifier_ReturnsSystemNotifier()
    {
        // Arrange/Act
        var notifier = _inputSystem.Notifier;

        // Assert
        Assert.NotNull(notifier);
        Assert.Same(_mockSystemNotifier.Object, notifier);
    }

    #endregion

    #region UserManager

    [Fact]
    public void UserManager_ReturnsUserManager()
    {
        // Arrange/Act
        var userManager = _inputSystem.UserManager;

        // Assert
        Assert.NotNull(userManager);
        Assert.Same(_mockUserManager.Object, userManager);
    }

    #endregion

    #region SchemeService

    [Fact]
    public void SchemeService_ReturnsSchemeService()
    {
        // Arrange/Act
        var schemeService = _inputSystem.SchemeService;

        // Assert
        Assert.NotNull(schemeService);
        Assert.Same(_mockSchemeService.Object, schemeService);
    }

    [Fact]
    public void AllowCustomSchemes_ReturnsSchemeServiceValue()
    {
        // Arrange
        _mockSchemeService.SetupGet(s => s.AllowCustomSchemes).Returns(true);

        // Act/Assert
        Assert.True(_inputSystem.AllowCustomSchemes);
    }

    #endregion

    #region PauseInput

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PauseInput_ReturnsInputServiceValue(bool isPaused)
    {
        // Arrange
        _mockInputService.SetupGet(m => m.PauseInput).Returns(isPaused);

        // Act/Assert
        Assert.Equal(isPaused, _inputSystem.PauseInput);
    }

    [Fact]
    public void PauseInput_SetToTrue_SetsInternalAndDelegates()
    {
        // Arrange/Act
        _inputSystem.PauseInput = true;

        // Assert
        _mockInputService.VerifySet(s => s.PauseInput = true, Times.Once);
    }

    [Fact]
    public void PauseInput_SetToFalse_SetsInternalAndDelegates()
    {
        // Arrange/Act
        _inputSystem.PauseInput = false;

        // Assert
        _mockInputService.VerifySet(s => s.PauseInput = false, Times.Once);
    }

    #endregion

    #region InitializeAsync

    [Fact]
    public async Task InitializeAsync_ValidConfiguration_ReturnsSuccess()
    {
        // Arrange
        _mockSchemeService.Setup(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success()));

        _mockInputService.Setup(s => s.InitializeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success()));

        // Act
        var result = await _inputSystem.InitializeAsync(new InputSystemConfiguration([], [], new(), new([])), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);

        _mockSchemeService.Verify(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockInputService.Verify(s => s.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_SchemeServiceReturnsError_ReturnsError()
    {
        // Arrange
        _mockSchemeService.Setup(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.InvalidRequest()));

        // Act
        var result = await _inputSystem.InitializeAsync(TestConfigurationHelper.CreateValidConfiguration(), TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);

        _mockSchemeService.Verify(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_InputServiceReturnsError_ReturnsError()
    {
        // Arrange
        _mockSchemeService.Setup(s => s.LoadSchemeConfigurationAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success()));

        _mockInputService.Setup(s => s.InitializeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.InvalidRequest()));

        // Act
        var result = await _inputSystem.InitializeAsync(TestConfigurationHelper.CreateValidConfiguration(), TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task InitializeAsync_InputSystemConfigurationProviderThrowsOnSet_ThrowsError()
    {
        // Arrange
        _mockConfigProvider
            .SetupSet(provider => provider.Configuration = It.IsAny<InputSystemConfiguration>())
            .Throws<InvalidOperationException>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _inputSystem.InitializeAsync(TestConfigurationHelper.CreateValidConfiguration(), TestContext.Current.CancellationToken));
    }

    #endregion

    #region Update

    [Fact]
    public void Update_WhenPaused_DoesNotDelegateToInputService()
    {
        // Arrange
        _mockInputService.SetupGet(m => m.PauseInput)
            .Returns(true);

        // Act
        _inputSystem.Update(TimeSpan.FromSeconds(1));

        // Assert
        _mockInputService.Verify(s => s.Update(It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public void Update_WhenNotPaused_DelegatesToInputService()
    {
        // Arrange
        _inputSystem.PauseInput = false;
        var delta = TimeSpan.FromSeconds(0.1);

        // Act
        _inputSystem.Update(delta);

        // Assert
        _mockInputService.Verify(s => s.Update(delta), Times.Once);
    }

    #endregion
}
