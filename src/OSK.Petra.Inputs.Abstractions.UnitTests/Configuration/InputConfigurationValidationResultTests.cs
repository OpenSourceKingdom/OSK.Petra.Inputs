using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class InputConfigurationValidationResultTests
{
    #region Success

    [Fact]
    public void Success_ReturnsValidResult()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.Success();

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(InputConfigurationValidation.Ok, result.Result);
        Assert.Equal("Ok", result.Message);
        Assert.Equal(ConfigurationType.InputSystem, result.ConfigurationType);
        Assert.Equal("Configuration", result.TargetName);
    }

    #endregion

    #region ForInputConfiguration

    [Fact]
    public void ForInputConfiguration_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForInputConfiguration(
            cfg => cfg.Schemes, InputConfigurationValidation.MissingData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.InputConfiguration, result.ConfigurationType);
        Assert.Equal("Schemes", result.TargetName);
        Assert.Equal(InputConfigurationValidation.MissingData, result.Result);
        Assert.Equal("Test message", result.Message);
    }

    #endregion

    #region ForInputSystem

    [Fact]
    public void ForInputSystem_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForInputSystem(
            cfg => cfg.JoinPolicy, InputConfigurationValidation.MissingData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.InputSystem, result.ConfigurationType);
        Assert.Equal("JoinPolicy", result.TargetName);
    }

    #endregion

    #region ForDefinition

    [Fact]
    public void ForDefinition_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForDefinition(
            def => def.Name, InputConfigurationValidation.InvalidData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.Definition, result.ConfigurationType);
        Assert.Equal("Name", result.TargetName);
    }

    #endregion

    #region ForInputAction

    [Fact]
    public void ForInputAction_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForInputAction(
            act => act.TriggerPhases, InputConfigurationValidation.MissingData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.InputAction, result.ConfigurationType);
        Assert.Equal("TriggerPhases", result.TargetName);
    }

    #endregion

    #region ForScheme

    [Fact]
    public void ForScheme_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForScheme(
            scheme => scheme.Name, InputConfigurationValidation.DuplicateData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.Scheme, result.ConfigurationType);
        Assert.Equal("Name", result.TargetName);
    }

    #endregion

    #region ForDeviceMap

    [Fact]
    public void ForDeviceMap_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForDeviceMap(
            map => map.DeviceIdentity, InputConfigurationValidation.InvalidData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.DeviceMap, result.ConfigurationType);
        Assert.Equal("DeviceIdentity", result.TargetName);
    }

    #endregion

    #region ForJoinPolicy

    [Fact]
    public void ForJoinPolicy_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForJoinPolicy(
            policy => policy.MaxUsers, InputConfigurationValidation.InvalidData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.JoinPolicy, result.ConfigurationType);
        Assert.Equal("MaxUsers", result.TargetName);
    }

    #endregion

    #region ForProcessorConfiguration

    [Fact]
    public void ForProcessorConfiguration_SetsCorrectType()
    {
        // Arrange & Act
        var result = InputConfigurationValidationResult.ForProcessorConfiguration(
            cfg => cfg.DeadzoneTolerance, InputConfigurationValidation.InvalidData, "Test message");

        // Assert
        Assert.Equal(ConfigurationType.InputProcessor, result.ConfigurationType);
        Assert.Equal("DeadzoneTolerance", result.TargetName);
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var result = InputConfigurationValidationResult.ForInputSystem(
            cfg => cfg.JoinPolicy, InputConfigurationValidation.MissingData, "Join Policy must exist.");

        // Act
        var str = result.ToString();

        // Assert
        Assert.Contains("InputSystem", str);
        Assert.Contains("JoinPolicy", str);
        Assert.Contains("MissingData", str);
        Assert.Contains("Join Policy must exist.", str);
    }

    #endregion
}
