using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Exceptions;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.UnitTests._Helpers;

namespace OSK.Petra.Inputs.UnitTests.Internal;

public class InputSystemConfigurationProviderTests
{
    #region Variables

    private readonly InputSystemConfigurationProvider _provider = new();

    #endregion

    #region Configuration (Get)

    [Fact]
    public void GetConfiguration_NullConfiguration_ThrowsInvalidOperationException()
    {
        // Arrange/Act/Assert
        Assert.Throws<InvalidOperationException>(() => _provider.Configuration);
    }

    #endregion

    #region Configuration (Set)

    [Fact]
    public void SetConfiguration_NullConfiguration_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _provider.Configuration = null!);
    }

    [Fact]
    public void SetConfiguration_ValidConfiguration_ReturnsSuccessfully()
    {
        // Arrange
        var config = TestConfigurationHelper.CreateValidConfiguration();

        // Act
        _provider.Configuration = config;

        // Assert
        Assert.Same(config, _provider.Configuration);
    }

    [Fact]
    public void SetConfiguration_InvalidConfiguration_ThrowsInputSystemValidationException()
    {
        // Arrange
        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = 0,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DevicePairingBehavior = DevicePairingBehavior.Balanced
        };
        var invalidConfig = new InputSystemConfiguration([], [], joinPolicy, new([]));

        // Act & Assert
        Assert.Throws<InputSystemValidationException>(() => _provider.Configuration = invalidConfig);
    }

    #endregion
}
