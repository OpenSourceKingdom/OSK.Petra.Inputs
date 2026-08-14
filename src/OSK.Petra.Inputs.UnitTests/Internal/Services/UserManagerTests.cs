using Microsoft.Extensions.Logging;
using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Options;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class UserManagerTests
{
    #region Variables

    private readonly Mock<IInputSystemConfigurationProvider> _mockConfigProvider;
    private readonly Mock<IInputSystemNotifier> _mockSystemNotifier;
    private readonly Mock<ISchemeRepository> _mockSchemeRepository;
    private readonly Mock<ILogger<UserManager>> _mockLogger;
    private readonly UserManager _userManager;

    #endregion

    #region Constructors

    public UserManagerTests()
    {
        var config = TestConfigurationHelper.CreateValidConfiguration(4);
        _mockConfigProvider = new Mock<IInputSystemConfigurationProvider>();
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(config);

        _mockSystemNotifier = new Mock<IInputSystemNotifier>();
        _mockSchemeRepository = new Mock<ISchemeRepository>();
        _mockSchemeRepository.SetupGet(m => m.AllowCustomSchemes).Returns(false);
        _mockLogger = new Mock<ILogger<UserManager>>();

        _userManager = new UserManager(
            _mockConfigProvider.Object,
            _mockSystemNotifier.Object,
            _mockSchemeRepository.Object,
            _mockLogger.Object);
    }

    #endregion

    #region Constructor_NullChecks

    [Fact]
    public void Constructor_NullConfigurationProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserManager(
            null!,
            _mockSystemNotifier.Object,
            _mockSchemeRepository.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullSystemNotifier_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserManager(
            _mockConfigProvider.Object,
            null!,
            _mockSchemeRepository.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullSchemeRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserManager(
            _mockConfigProvider.Object,
            _mockSystemNotifier.Object,
            null!,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserManager(
            _mockConfigProvider.Object,
            _mockSystemNotifier.Object,
            _mockSchemeRepository.Object,
            null!));
    }

    #endregion

    #region CreateUser

    [Fact]
    public void CreateUser_NullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _userManager.CreateUser(null!));
    }

    [Fact]
    public void CreateUser_MaxUsersReached_ReturnsError()
    {
        // Arrange
        var config = TestConfigurationHelper.CreateValidConfiguration(2);
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(config);

        var userManager = new UserManager(
            _mockConfigProvider.Object,
            _mockSystemNotifier.Object,
            _mockSchemeRepository.Object,
            _mockLogger.Object);

        userManager.CreateUser(new UserJoinOptions());
        userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = userManager.CreateUser(new UserJoinOptions());

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void CreateUser_FirstUser_GetsIdOne()
    {
        // Arrange
        var options = new UserJoinOptions();

        // Act
        var result = _userManager.CreateUser(options);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public void CreateUser_ExistingUsers_GetsIncrementedId()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());
        var options = new UserJoinOptions();

        // Act
        var result = _userManager.CreateUser(options);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(2, result.Data.Id);
    }

    [Fact]
    public void CreateUser_RaisesUserJoinedNotification()
    {
        // Arrange
        var options = new UserJoinOptions();

        // Act
        _userManager.CreateUser(options);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UserJoinedNotification>(x => true)), Times.Once);
    }

    [Fact]
    public void CreateUser_WithDevicesToPair_PairsDevices()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var options = new UserJoinOptions() { DevicesToPair = [device] };

        // Act
        var result = _userManager.CreateUser(options);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data.PairedDevices);
    }

    [Fact]
    public void CreateUser_WithDevicesToPair_RaisesDevicePairedNotification()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var options = new UserJoinOptions() { DevicesToPair = [device] };

        // Act
        _userManager.CreateUser(options);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<DevicePairedNotification>(x => true)), Times.Once);
    }

    [Fact]
    public void CreateUser_WithAlreadyPairedDevice_ReturnsInvalidRequest()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 500);
        _userManager.CreateUser(new UserJoinOptions() { DevicesToPair = [device] });

        var pairedDevice = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 500);
        var options = new UserJoinOptions() { DevicesToPair = [pairedDevice] };

        // Act
        var result = _userManager.CreateUser(options);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void CreateUser_WithActiveDefinitionName_UsesThatDefinition()
    {
        // Arrange
        var actions = new[] { new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }) };
        var def1 = new ActionDefinition("Default", actions, isDefault: true);
        var def2 = new ActionDefinition("Secondary", actions, isDefault: false);

        var scheme = new InputScheme("Default", "Default", [], isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 4,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced
        };

        var mockTopology = new Mock<IDeviceTopology>();
        mockTopology.SetupGet(m => m.Name).Returns(DeviceTopologyName.Keyboard);
        mockTopology.Setup(m => m.IsCompatibleInput(It.IsAny<IInput>())).Returns(true);

        var configProvider = new Mock<IInputSystemConfigurationProvider>();
        configProvider.SetupGet(m => m.Configuration).Returns(new InputSystemConfiguration([mockTopology.Object], new[] { config }, new[] { def1, def2 }, joinPolicy));

        var userManager = new UserManager(
            configProvider.Object,
            _mockSystemNotifier.Object,
            _mockSchemeRepository.Object,
            _mockLogger.Object);

        // Act
        var result = userManager.CreateUser(new UserJoinOptions() { ActiveDefinitionName = "Secondary" });

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal("Secondary", result.Data.ActiveDefinitionName);
    }

    [Fact]
    public void CreateUser_WithNonExistentActiveDefinition_FallsBackToDefault()
    {
        // Arrange
        var options = new UserJoinOptions() { ActiveDefinitionName = "NonExistent" };

        // Act
        var result = _userManager.CreateUser(options);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region SetActiveDefinition

    [Fact]
    public void SetActiveDefinition_UserNotFound_ReturnsDataNotFound()
    {
        // Act
        var result = _userManager.SetActiveDefinition(999, "Default");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetActiveDefinition_EmptyDefinitionName_ReturnsInvalidRequest()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.SetActiveDefinition(1, "");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetActiveDefinition_NullDefinitionName_ReturnsInvalidRequest()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.SetActiveDefinition(1, null!);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetActiveDefinition_NonExistentDefinition_ReturnsDataNotFound()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.SetActiveDefinition(1, "NonExistent");

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void SetActiveDefinition_ValidDefinition_SetsDefinitionAndNotifies()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.SetActiveDefinition(1, "Default");

        // Assert
        Assert.True(result.IsSuccessful);
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UserActiveDefinitionChangeNotification>(x => true)), Times.Once);
    }

    #endregion

    #region GetUserForDevice

    [Fact]
    public void GetUserForDevice_NoUsers_ReturnsNull()
    {
        // Act
        var result = _userManager.GetUserForDevice(100);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserForDevice_DeviceNotPaired_ReturnsNull()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.GetUserForDevice(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserForDevice_DevicePaired_ReturnsUser()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var createResult = _userManager.CreateUser(new UserJoinOptions() { DevicesToPair = [device] });

        // Act
        var result = _userManager.GetUserForDevice(device.DeviceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createResult.Data.Id, result.Id);
    }

    #endregion

    #region GetUser

    [Fact]
    public void GetUser_UserNotFound_ReturnsNull()
    {
        // Act
        var result = _userManager.GetUser(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUser_UserExists_ReturnsUser()
    {
        // Arrange
        var createUserResult = _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.GetUser(createUserResult.Data.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createUserResult.Data.Id, result.Id);
    }

    #endregion

    #region GetUsers

    [Fact]
    public void GetUsers_NoUsers_ReturnsEmpty()
    {
        // Act
        var result = _userManager.GetUsers();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetUsers_WithUsers_ReturnsAllUsers()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.GetUsers();

        // Assert
        Assert.Equal(2, result.Count());
    }

    #endregion

    #region RemoveUser

    [Fact]
    public void RemoveUser_UserNotFound_ReturnsFalse()
    {
        // Act
        var result = _userManager.RemoveUser(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RemoveUser_UserExists_RemovesAndNotifies()
    {
        // Arrange
        var createUserResult = _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.RemoveUser(createUserResult.Data.Id);

        // Assert
        Assert.True(result);
        Assert.Null(_userManager.GetUser(createUserResult.Data.Id));
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<UserRemovedNotification>(x => true)), Times.Once);
    }

    [Fact]
    public void RemoveUser_UserWithPairedDevices_UnpairsAllDevices()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        _userManager.CreateUser(new UserJoinOptions() { DevicesToPair = [device] });

        // Act
        _userManager.RemoveUser(1);

        // Assert
        _mockSystemNotifier.Verify(n => n.Notify(It.Is<DeviceUnpairedNotification>(x => true)), Times.Once);
    }

    #endregion

    #region PairDevice

    [Fact]
    public void PairDevice_UserNotFound_ReturnsDataNotFound()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);

        // Act
        var result = _userManager.PairDevice(999, device);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void PairDevice_DeviceAlreadyPairedToOtherUser_ReturnsInvalidRequest()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        _userManager.CreateUser(new UserJoinOptions() { DevicesToPair = [device] });
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.PairDevice(2, device);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void PairDevice_DeviceAlreadyPairedToSameUser_ReturnsSuccess()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        var createUserResult = _userManager.CreateUser(new UserJoinOptions() { DevicesToPair = [device] });

        // Act
        var result = _userManager.PairDevice(createUserResult.Data.Id, device);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void PairDevice_ValidPairing_AddsToDeviceList()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard, deviceId: 800);

        // Act
        var result = _userManager.PairDevice(1, device);

        // Assert
        Assert.True(result.IsSuccessful);
        var user = _userManager.GetUser(1);
        Assert.NotNull(user);
        Assert.Single(user!.PairedDevices);
    }

    #endregion

    #region UnpairDevice

    [Fact]
    public void UnpairDevice_UserNotFound_ReturnsFalse()
    {
        // Act
        var result = _userManager.UnpairDevice(999, 100);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UnpairDevice_DeviceNotPaired_ReturnsFalse()
    {
        // Arrange
        _userManager.CreateUser(new UserJoinOptions());

        // Act
        var result = _userManager.UnpairDevice(1, 999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UnpairDevice_ValidUnpairing_RemovesDeviceAndNotifies()
    {
        // Arrange
        var device = TestConfigurationHelper.CreateDeviceIdentifier(DeviceTopologyName.Keyboard);
        _userManager.CreateUser(new UserJoinOptions() { DevicesToPair = [device] });

        // Act
        var result = _userManager.UnpairDevice(1, device.DeviceId);

        // Assert
        Assert.True(result);
        var user = _userManager.GetUser(1);
        Assert.NotNull(user);
        Assert.Empty(user!.PairedDevices);
    }

    #endregion

    #region SavePreferredSchemeAsync

    [Fact]
    public async Task SavePreferredSchemeAsync_UserIdOutOfRange_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = -1, DefinitionName = "Default", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = await _userManager.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_EmptyDefinitionName_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = await _userManager.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_NonExistentDefinition_ReturnsDataNotFound()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "NonExistent", SchemeName = "Test", ConfigurationId = "1" };

        // Act
        var result = await _userManager.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_EmptySchemeName_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "", ConfigurationId = "1" };

        // Act
        var result = await _userManager.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_EmptyConfigurationId_ReturnsInvalidRequest()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Test", ConfigurationId = "" };

        // Act
        var result = await _userManager.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_NonExistentConfigurationId_ReturnsDataNotFound()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Test", ConfigurationId = "nonexistent" };

        // Act
        var result = await _userManager.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_ValidScheme_DelegatesToRepository()
    {
        // Arrange
        var scheme = new PreferredInputScheme() {
            UserId = 1, 
            DefinitionName = "Default", 
            SchemeName = "Default", 
            ConfigurationId = InputConfiguration.GetConfigurationId(DeviceTopologyName.Keyboard, DeviceTopologyName.Mouse) 
        };
        _mockSchemeRepository.Setup(r => r.SavePreferredSchemeAsync(scheme, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success(scheme)));

        // Act
        var result = await _userManager.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        _mockSchemeRepository.Verify(r => r.SavePreferredSchemeAsync(scheme, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
