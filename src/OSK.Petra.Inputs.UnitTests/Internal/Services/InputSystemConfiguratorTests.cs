using Microsoft.Extensions.DependencyInjection;
using Moq;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Exceptions;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class InputSystemConfiguratorTests
{
    #region Variables

    private readonly InputSystemConfigurator _configurator;

    #endregion

    #region Constructors

    public InputSystemConfiguratorTests()
    {
        _configurator = new InputSystemConfigurator();
    }

    #endregion

    #region UseConfiguration

    [Fact]
    public void UseConfiguration_NullConfiguration_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _configurator.UseConfiguration(null!));
    }

    [Fact]
    public void UseConfiguration_ValidConfiguration_ReturnsSelf()
    {
        // Arrange
        var config = TestConfigurationHelper.CreateValidConfiguration();

        // Act
        var result = _configurator.UseConfiguration(config);

        // Assert
        Assert.Same(_configurator, result);
    }

    [Fact]
    public void UseConfiguration_InvalidConfiguration_ThrowsInputSystemValidationException()
    {
        // Arrange
        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 0,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced
        };
        var invalidConfig = new InputSystemConfiguration([], [], joinPolicy);

        // Act & Assert
        Assert.Throws<InputSystemValidationException>(() => _configurator.UseConfiguration(invalidConfig));
    }

    #endregion

    #region UseSchemeRepository

    [Fact]
    public void UseSchemeRepository_NullType_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _configurator.UseSchemeRepository(null!));
    }

    [Fact]
    public void UseSchemeRepository_NonImplementingType_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _configurator.UseSchemeRepository(typeof(string)));
        Assert.Contains("ISchemeRepository", ex.Message);
    }

    [Fact]
    public void UseSchemeRepository_ValidType_ReturnsSelf()
    {
        // Arrange
        var mockRepo = new Mock<ISchemeRepository>();

        // Act
        var result = _configurator.UseSchemeRepository(mockRepo.Object.GetType());

        // Assert
        Assert.Same(_configurator, result);
    }

    #endregion

    #region WithDeviceProvider

    [Fact]
    public void WithDeviceProvider_ValidProvider_ReturnsSelf()
    {
        // Arrange
        var mockProvider = new Mock<IDeviceProvider>();

        // Act
        var result = _configurator.WithDeviceProvider<MockDeviceProvider>();

        // Assert
        Assert.Same(_configurator, result);
    }

    #endregion

    #region ConfigureServices

    [Fact]
    public void ConfigureServices_NoConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _configurator.ConfigureServices(services));
    }

    [Fact]
    public void ConfigureServices_Default_SetsExpectedServices()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationHelper.CreateValidConfiguration());
        var services = new ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var configurationProviderDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputSystemConfigurationProvider));
        Assert.NotNull(configurationProviderDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, configurationProviderDescriptor.Lifetime);

        var schemeServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ISchemeService));
        Assert.NotNull(schemeServiceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, schemeServiceDescriptor.Lifetime);

        var inputServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputService));
        Assert.NotNull(inputServiceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, inputServiceDescriptor.Lifetime);

        var usermanagerDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserManager));
        Assert.NotNull(usermanagerDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, usermanagerDescriptor.Lifetime);

        var inputSystemDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputSystem));
        Assert.NotNull(inputSystemDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, inputSystemDescriptor.Lifetime);

        var systemNotifierDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputSystemNotifier));
        Assert.NotNull(systemNotifierDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, systemNotifierDescriptor.Lifetime);

        var catalogProviderDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDeviceCatalogProvider));
        Assert.NotNull(catalogProviderDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, catalogProviderDescriptor.Lifetime);

        var defaultSchemeRepositoryDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ISchemeRepository));
        Assert.NotNull(defaultSchemeRepositoryDescriptor);
        Assert.Equal(typeof(InMemorySchemeRepository), defaultSchemeRepositoryDescriptor.ImplementationType);
    }

    [Fact]
    public void ConfigureServices_WithCustomSchemeRepository_UsesCustomType()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationHelper.CreateValidConfiguration());
        var services = new ServiceCollection();

        // Act
        _configurator.UseSchemeRepository(typeof(TestSchemeRepository));
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ISchemeRepository));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(TestSchemeRepository), descriptor.ImplementationType);
    }

    [Fact]
    public void ConfigureServices_WithDeviceProvider_RegistersTransient()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationHelper.CreateValidConfiguration());
        var services = new ServiceCollection();

        // Act
        _configurator.WithDeviceProvider<MockDeviceProvider>();
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDeviceProvider));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    #endregion
}
