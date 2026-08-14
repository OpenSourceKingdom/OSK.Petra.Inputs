using Microsoft.Extensions.Logging;
using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Models;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class SchemeServiceTests
{
    #region Variables

    private readonly Mock<IInputSystemConfigurationProvider> _mockConfigProvider;
    private readonly Mock<ISchemeRepository> _mockSchemeRepository;
    private readonly Mock<IUserManager> _mockUserManager;
    private readonly Mock<IInputSystemNotifier> _mockSystemNotifier;
    private readonly Mock<IDeviceCatalogProvider> _mockDeviceCatalogProvider;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ILogger<SchemeService>> _mockLogger;
    private readonly InputSystemConfiguration _validConfig;

    #endregion

    #region Constructors

    public SchemeServiceTests()
    {
        _validConfig = TestConfigurationHelper.CreateValidConfiguration();
        _mockConfigProvider = new Mock<IInputSystemConfigurationProvider>();
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(_validConfig);

        _mockSchemeRepository = new Mock<ISchemeRepository>();
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(false);
        _mockUserManager = new Mock<IUserManager>();
        _mockSystemNotifier = new Mock<IInputSystemNotifier>();
        _mockDeviceCatalogProvider = new Mock<IDeviceCatalogProvider>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<SchemeService>>();
    }

    private SchemeService CreateService()
    {
        return new SchemeService(
            _mockConfigProvider.Object,
            _mockSchemeRepository.Object,
            _mockUserManager.Object,
            _mockSystemNotifier.Object,
            _mockDeviceCatalogProvider.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object);
    }

    #endregion

    #region AllowCustomSchemes

    [Fact]
    public void AllowCustomSchemes_RepositoryAllows_ReturnsTrue()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        var service = CreateService();

        // Act
        Assert.True(service.AllowCustomSchemes);
    }

    [Fact]
    public void AllowCustomSchemes_RepositoryDenies_ReturnsFalse()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(false);
        var service = CreateService();

        // Act
        Assert.False(service.AllowCustomSchemes);
    }

    #endregion

    #region GetPreferredInputScheme

    [Fact]
    public void GetPreferredInputScheme_NoPreference_ReturnsNull()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetPreferredInputScheme(1, "config-id", "Default");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPreferredInputScheme_EmptyConfigurationId_ReturnsNull()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetPreferredInputScheme(1, "", "Default");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPreferredInputScheme_EmptyDefinitionName_ReturnsNull()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetPreferredInputScheme(1, "config-id", "");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPreferredInputScheme_NullDefinitionName_ReturnsNull()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetPreferredInputScheme(1, "config-id", null!);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetActiveSchemeForUser

    [Fact]
    public void GetActiveSchemeForUser_UserNotFound_ReturnsNull()
    {
        // Arrange
        _mockUserManager.Setup(m => m.GetUser(1)).Returns((IInputUser?)null);
        var service = CreateService();

        // Act
        var result = service.GetActiveSchemeForUser(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetActiveSchemeForUser_NoActiveScheme_ReturnsNull()
    {
        // Arrange
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(new InputUser(1));
        var service = CreateService();

        // Act
        var result = service.GetActiveSchemeForUser(1);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region SetActiveSchemeForDevice

    [Fact]
    public void SetActiveSchemeForDevice_UserNotFound_ReturnsDataNotFound()
    {
        // Arrange
        _mockUserManager.Setup(m => m.GetUser(1)).Returns((IInputUser?)null);
        var service = CreateService();

        // Act
        var result = service.SetActiveSchemeForDevice(1, new DeviceIdentity());

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetActiveSchemeForDevice_AlreadyActiveForTopology_ReturnsSameScheme()
    {
        // Arrange
        var user = new InputUser(1);
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(user);

        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        var service = CreateService();

        // Simulate active scheme already set
        var activeSchemesField = typeof(SchemeService).GetField("_activeUserSchemesLookup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lookup = (Dictionary<int, InputScheme>)activeSchemesField!.GetValue(service)!;
        lookup[1] = scheme;

        // Act
        var result = service.SetActiveSchemeForDevice(1, new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test"));

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void SetActiveSchemeForDevice_NoMatchingConfig_ReturnsInvalidRequest()
    {
        // Arrange
        var user = new InputUser(1);
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(user);

        var configProvider = new Mock<IInputSystemConfigurationProvider>();
        configProvider.SetupGet(m => m.Configuration).Returns(TestConfigurationHelper.CreateValidConfiguration());

        var service = new SchemeService(
            configProvider.Object,
            _mockSchemeRepository.Object,
            _mockUserManager.Object,
            _mockSystemNotifier.Object,
            _mockDeviceCatalogProvider.Object,
            _mockServiceProvider.Object,
            _mockLogger.Object);

        // Act
        var result = service.SetActiveSchemeForDevice(1, new DeviceIdentity(DeviceTopologyName.Gamepad, DeviceFamily.Generic, "Gamepad"));

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetActiveSchemeForDevice_ValidDevice_SetsActiveSchemeAndNotifies()
    {
        // Arrange
        var user = new InputUser(1);
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(user);

        var service = CreateService();
        var deviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Test");

        // Act
        var result = service.SetActiveSchemeForDevice(1, deviceIdentity);

        // Assert
        Assert.True(result.IsSuccessful);
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UserActiveSchemeChangeNotification>(x => true)), Times.Once);
    }

    #endregion

    #region SavePreferredSchemeAsync

    [Fact]
    public async Task SavePreferredSchemeAsync_UserIdOutOfRange_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = -1, DefinitionName = "Default", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = await service.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_EmptyDefinitionName_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = await service.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_NonExistentDefinition_ReturnsDataNotFound()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "NonExistent", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = await service.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_EmptySchemeName_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "", ConfigurationId = "1" };

        // Act
        var result = await service.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_NonExistentConfiguration_ReturnsDataNotFound()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Test", ConfigurationId = "nonexistent" };

        // Act
        var result = await service.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_ValidScheme_DelegatesToRepository()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Default", ConfigurationId = InputConfiguration.GetConfigurationId(DeviceTopologyName.Keyboard, DeviceTopologyName.Mouse) };
        _mockSchemeRepository.Setup(r => r.SavePreferredSchemeAsync(scheme, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success(scheme)));

        var service = CreateService();

        // Act
        var result = await service.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region SaveCustomSchemeAsync

    [Fact]
    public async Task SaveCustomSchemeAsync_NullScheme_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveCustomSchemeAsync(null!, SchemeSavePermissions.Overwrite, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SaveCustomSchemeAsync_CustomSchemesNotAllowed_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new CustomInputScheme() { DefinitionName = "Default", Name = "Test", DeviceMaps = [] };

        // Act
        var result = await service.SaveCustomSchemeAsync(scheme, SchemeSavePermissions.Overwrite, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SaveCustomSchemeAsync_CustomSchemesAllowed_DelegatesToRepository()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        _mockSchemeRepository.Setup(r => r.SaveCustomInputScheme(It.IsAny<CustomInputScheme>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success(Mock.Of<CustomInputScheme>())));

        var service = CreateService();
        var scheme = new CustomInputScheme() 
        { 
            DefinitionName = "Default", 
            Name = "Test", 
            DeviceMaps = [
                new DeviceInputMap() 
                { 
                    DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Generic"),
                    InputMaps = [
                         new InputActionMap(new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }, "Moves the cursor"), Mock.Of<IInput>()),
                         new InputActionMap(new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start, InputPhase.End }, ctx => { }, "Clicks"), Mock.Of<IInput>())
                    ]
                },
                new DeviceInputMap()
                {
                    DeviceIdentity = new DeviceIdentity(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Generic"),
                    InputMaps = []
                }
            ] 
        };

        // Act
        var result = await service.SaveCustomSchemeAsync(scheme, SchemeSavePermissions.Overwrite, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region DeleteCustomSchemeAsync

    [Fact]
    public async Task DeleteCustomSchemeAsync_CustomSchemesNotAllowed_ReturnsSuccess()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.DeleteCustomSchemeAsync("Default", "Test", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task DeleteCustomSchemeAsync_EmptyDefinitionName_ReturnsSuccess()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        var service = CreateService();

        // Act
        var result = await service.DeleteCustomSchemeAsync("", "Test", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task DeleteCustomSchemeAsync_EmptySchemeName_ReturnsSuccess()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        var service = CreateService();

        // Act
        var result = await service.DeleteCustomSchemeAsync("Default", "", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task DeleteCustomSchemeAsync_NonExistentDefinition_ReturnsSuccess()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        var service = CreateService();

        // Act
        var result = await service.DeleteCustomSchemeAsync("NonExistent", "Test", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task DeleteCustomSchemeAsync_ValidRequest_DelegatesToRepository()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        _mockSchemeRepository.Setup(r => r.DeleteCustomSchemeAsync("Default", "Test", It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success()));

        var service = CreateService();

        // Act
        var result = await service.DeleteCustomSchemeAsync("Default", "Test", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region GetInputSchemes

    [Fact]
    public void GetInputSchemes_NoSchemes_ReturnsEmpty()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetInputSchemes("nonexistent", "Default");

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetSchemeEditorAsync

    [Fact]
    public async Task GetSchemeEditorAsync_UserNotFound_ReturnsDataNotFound()
    {
        // Arrange
        _mockUserManager.Setup(m => m.GetUser(1)).Returns((IInputUser?)null);
        var service = CreateService();

        // Act
        var result = await service.GetSchemeEditorAsync(1, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task GetSchemeEditorAsync_DeviceCatalogFails_ReturnsError()
    {
        // Arrange
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(new InputUser(1));
        _mockDeviceCatalogProvider.Setup(p => p.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.InvalidRequest<DeviceCatalog>("catalog error")));

        var service = CreateService();

        // Act
        var result = await service.GetSchemeEditorAsync(1, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    #endregion
}
