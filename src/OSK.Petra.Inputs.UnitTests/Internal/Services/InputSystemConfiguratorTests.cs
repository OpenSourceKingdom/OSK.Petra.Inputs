using Moq;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Exceptions;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class InputSystemConfiguratorTests
{
    #region Variables

    private readonly InputSystemConfigurator _configurator;
    private readonly InputSystemConfiguration _validConfig;

    #endregion

    #region Constructors

    public InputSystemConfiguratorTests()
    {
        _validConfig = TestConfigurationFactory.CreateValidConfiguration();
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
        var config = TestConfigurationFactory.CreateValidConfiguration();

        // Act
        var result = _configurator.UseConfiguration(config);

        // Assert
        Assert.Same(_configurator, result);
    }

    [Fact]
    public void UseConfiguration_InvalidConfiguration_ThrowsInputSystemValidationException()
    {
        // Arrange
        var invalidConfig = CreateInvalidConfiguration();

        // Act & Assert
        Assert.Throws<InputSystemValidationException>(() => _configurator.UseConfiguration(invalidConfig));
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

    [Fact]
    public void UseSchemeRepository_InMemorySchemeRepository_SetsDefault()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());

        // Act - use InMemorySchemeRepository type
        var result = _configurator.UseSchemeRepository(typeof(InMemorySchemeRepository));

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

    [Fact]
    public void WithDeviceProvider_Chainable_AllowsMultipleCalls()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());

        // Act
        var result = _configurator
            .WithDeviceProvider<MockDeviceProvider>()
            .UseSchemeRepository(typeof(InMemorySchemeRepository));

        // Assert
        Assert.Same(_configurator, result);
    }

    #endregion

    #region ConfigureServices

    [Fact]
    public void ConfigureServices_NoConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _configurator.ConfigureServices(services));
    }

    [Fact]
    public void ConfigureServices_SetsConfigurationProviderSingleton()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputSystemConfigurationProvider));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_SetsSchemeServiceSingleton()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ISchemeService));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_SetsInputServiceSingleton()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputService));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_SetsUserManagerSingleton()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserManager));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_SetsInputSystemSingleton()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputSystem));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_SetsInputSystemNotifierSingleton()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInputSystemNotifier));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_SetsDeviceCatalogProviderSingleton()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDeviceCatalogProvider));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_UsesInMemorySchemeRepositoryByDefault()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ISchemeRepository));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(InMemorySchemeRepository), descriptor.ImplementationType);
    }

    [Fact]
    public void ConfigureServices_WithCustomSchemeRepository_UsesCustomType()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var customRepo = new Mock<ISchemeRepository>();
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.UseSchemeRepository(customRepo.Object.GetType());
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ISchemeRepository));
        Assert.NotNull(descriptor);
        Assert.Equal(customRepo.Object.GetType(), descriptor.ImplementationType);
    }

    [Fact]
    public void ConfigureServices_WithDeviceProvider_RegistersTransient()
    {
        // Arrange
        _configurator.UseConfiguration(TestConfigurationFactory.CreateValidConfiguration());
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        _configurator.WithDeviceProvider<MockDeviceProvider>();
        _configurator.ConfigureServices(services);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDeviceProvider));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient, descriptor.Lifetime);
    }

    #endregion
}
