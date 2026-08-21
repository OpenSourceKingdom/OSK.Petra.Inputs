using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Extensions.Petra.Inputs.Configuration.UnitTests._Helpers;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests;

public class InputSystemBuilderExtensionsTests
{
    #region Variables

    private readonly InputSystemBuilder _builder;

    #endregion

    #region Constructors

    public InputSystemBuilderExtensionsTests()
    {
        _builder = new InputSystemBuilder();
    }

    #endregion

    #region WithDefinition

    [Fact]
    public void WithDefinition_NullType_ThrowsArgumentNullException()
    {
        // Arrange
        Type? type = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithDefinition("Test", type!));
    }

    [Fact]
    public void WithDefinition_FindsMethodsMarkedWithAttribute()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        Assert.NotNull(foundDefinition);
        Assert.Contains("CustomName", foundDefinition.Actions.Select(a => a.Name));
    }

    [Fact]
    public void WithDefinition_AttributeMethod_UsesCustomActionName()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        var customAction = foundDefinition?.Actions.FirstOrDefault(a => a.Name == "CustomName");
        Assert.NotNull(customAction);
    }

    [Fact]
    public void WithDefinition_AttributeMethod_UsesAttributeTriggerPhases()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        var action = foundDefinition?.Actions.FirstOrDefault(a => a.Name == "CustomName");
        Assert.NotNull(action);
        Assert.Contains(InputPhase.Start, action!.TriggerPhases);
        Assert.Contains(InputPhase.End, action.TriggerPhases);
    }

    [Fact]
    public void WithDefinition_FindsUnmarkedMethods()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        Assert.NotNull(foundDefinition);
        Assert.Contains("UnmarkedMethod", foundDefinition.Actions.Select(a => a.Name));
    }

    [Fact]
    public void WithDefinition_UnmarkedMethod_UsesMethodNameAsActionName()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        var action = foundDefinition?.Actions.FirstOrDefault(a => a.Name == "UnmarkedMethod");
        Assert.NotNull(action);
    }

    [Fact]
    public void WithDefinition_UnmarkedMethod_DefaultsToStartPhase()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        var action = foundDefinition?.Actions.FirstOrDefault(a => a.Name == "UnmarkedMethod");
        Assert.NotNull(action);
        Assert.Contains(InputPhase.Start, action!.TriggerPhases);
    }

    [Fact]
    public void WithDefinition_MethodsWithWrongReturnType_Excluded()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        Assert.NotNull(foundDefinition);
        Assert.DoesNotContain("MethodWithReturnValue", foundDefinition.Actions.Select(a => a.Name));
    }

    [Fact]
    public void WithDefinition_MethodsWithWrongParameterCount_Excluded()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        Assert.NotNull(foundDefinition);
        Assert.DoesNotContain("MethodWithTwoParams", foundDefinition.Actions.Select(a => a.Name));
    }

    [Fact]
    public void WithDefinition_MethodsWithWrongParameterType_Excluded()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition("Test", typeof(TestDefinition));

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        Assert.NotNull(foundDefinition);
        Assert.DoesNotContain("MethodWithWrongParamType", foundDefinition.Actions.Select(a => a.Name));
    }

    #endregion

    #region WithDefinition (T)

    [Fact]
    public void WithDefinition_Generic_DelegatesToNonGeneric()
    {
        // Arrange
        var definition = new ActionDefinition("Before", [], isDefault: false);
        _builder.WithActionDefinition(definition);

        // Act
        _builder.WithDefinition<TestDefinition>("Test");

        // Assert
        var config = _builder.BuildConfiguration();
        var foundDefinition = config.GetDefinition("Test");
        Assert.NotNull(foundDefinition);
    }

    #endregion
}
