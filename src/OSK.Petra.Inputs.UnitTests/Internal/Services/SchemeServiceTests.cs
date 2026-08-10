using Microsoft.Extensions.Logging;
using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal;
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
        _validConfig = TestConfigurationFactory.CreateValidConfiguration();
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
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(TestConfigurationFactory.CreateUser(1));
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
        var user = TestConfigurationFactory.CreateUser(1);
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
        var user = TestConfigurationFactory.CreateUser(1);
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(user);

        var configProvider = new Mock<IInputSystemConfigurationProvider>();
        configProvider.SetupGet(m => m.Configuration).Returns(TestConfigurationFactory.CreateValidConfiguration());
        configProvider.Setup(m => m.Configuration.GetBestFitInputConfiguration(It.IsAny<DeviceIdentity>())).Returns((InputConfiguration?)null);

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
        var user = TestConfigurationFactory.CreateUser(1);
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
    public void SavePreferredSchemeAsync_UserIdOutOfRange_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = -1, DefinitionName = "Default", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = service.SavePreferredSchemeAsync(scheme).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SavePreferredSchemeAsync_EmptyDefinitionName_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = service.SavePreferredSchemeAsync(scheme).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SavePreferredSchemeAsync_NonExistentDefinition_ReturnsDataNotFound()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "NonExistent", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = service.SavePreferredSchemeAsync(scheme).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SavePreferredSchemeAsync_EmptySchemeName_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "", ConfigurationId = "1" };

        // Act
        var result = service.SavePreferredSchemeAsync(scheme).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SavePreferredSchemeAsync_NonExistentConfiguration_ReturnsDataNotFound()
    {
        // Arrange
        var service = CreateService();
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Test", ConfigurationId = "nonexistent" };

        // Act
        var result = service.SavePreferredSchemeAsync(scheme).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SavePreferredSchemeAsync_ValidScheme_DelegatesToRepository()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Default", ConfigurationId = "1" };
        _mockSchemeRepository.Setup(r => r.SavePreferredSchemeAsync(scheme, default))
            .Returns(Task.FromResult(Out.Success(scheme)));

        var service = CreateService();

        // Act
        var result = service.SavePreferredSchemeAsync(scheme).GetAwaiter().GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region SaveCustomSchemeAsync

    [Fact]
    public void SaveCustomSchemeAsync_NullScheme_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.SaveCustomSchemeAsync(null!, SchemeSavePermissions.Overwrite).GetAwaiter().GetResult());
    }

    [Fact]
    public void SaveCustomSchemeAsync_CustomSchemesNotAllowed_ReturnsInvalidRequest()
    {
        // Arrange
        var service = CreateService();
        var scheme = new CustomInputScheme() { DefinitionName = "Default", Name = "Test", DeviceMaps = [] };

        // Act
        var result = service.SaveCustomSchemeAsync(scheme, SchemeSavePermissions.Overwrite).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SaveCustomSchemeAsync_CustomSchemesAllowed_DelegatesToRepository()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        _mockSchemeRepository.Setup(r => r.SaveCustomInputScheme(It.IsAny<CustomInputScheme>(), default))
            .Returns(Task.FromResult(Out.Success(Mock.Of<CustomInputScheme>())));

        var service = CreateService();
        var scheme = new CustomInputScheme() { DefinitionName = "Default", Name = "Test", DeviceMaps = [] };

        // Act
        var result = service.SaveCustomSchemeAsync(scheme, SchemeSavePermissions.Overwrite).GetAwaiter().GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region DeleteCustomSchemeAsync

    [Fact]
    public void DeleteCustomSchemeAsync_CustomSchemesNotAllowed_ReturnsSuccess()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.DeleteCustomSchemeAsync("Default", "Test").GetAwaiter().GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DeleteCustomSchemeAsync_EmptyDefinitionName_ReturnsSuccess()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        var service = CreateService();

        // Act
        var result = service.DeleteCustomSchemeAsync("", "Test").GetAwaiter().GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DeleteCustomSchemeAsync_EmptySchemeName_ReturnsSuccess()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        var service = CreateService();

        // Act
        var result = service.DeleteCustomSchemeAsync("Default", "").GetAwaiter().GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DeleteCustomSchemeAsync_NonExistentDefinition_ReturnsSuccess()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        var service = CreateService();

        // Act
        var result = service.DeleteCustomSchemeAsync("NonExistent", "Test").GetAwaiter().GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DeleteCustomSchemeAsync_ValidRequest_DelegatesToRepository()
    {
        // Arrange
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(true);
        _mockSchemeRepository.Setup(r => r.DeleteCustomSchemeAsync("Default", "Test", default))
            .Returns(Task.FromResult(Out.Success()));

        var service = CreateService();

        // Act
        var result = service.DeleteCustomSchemeAsync("Default", "Test").GetAwaiter().GetResult();

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
    public void GetSchemeEditorAsync_UserNotFound_ReturnsDataNotFound()
    {
        // Arrange
        _mockUserManager.Setup(m => m.GetUser(1)).Returns((IInputUser?)null);
        var service = CreateService();

        // Act
        var result = service.GetSchemeEditorAsync(1).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void GetSchemeEditorAsync_DeviceCatalogFails_ReturnsError()
    {
        // Arrange
        _mockUserManager.Setup(m => m.GetUser(1)).Returns(TestConfigurationFactory.CreateUser(1));
        _mockDeviceCatalogProvider.Setup(p => p.GetCatalogAsync(default))
            .Returns(Task.FromResult(Out.InvalidRequest<DeviceCatalog>("catalog error")));

        var service = CreateService();

        // Act
        var result = service.GetSchemeEditorAsync(1).GetAwaiter().GetResult();

        // Assert
        Assert.False(result.IsSuccessful);
    }

    #endregion
}
