using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Extensions.Petra.Inputs.Configuration.UnitTests._Helpers;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests.Internal.Services;

public class DeviceMapBuilderTests
{
    #region Variables

    private readonly DeviceIdentity _testIdentity;
    private readonly DeviceMapBuilder _builder;
    private readonly TestInput _testInput1;
    private readonly TestInput _testInput2;

    #endregion

    #region Constructors

    public DeviceMapBuilderTests()
    {
        _testIdentity = new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "TestKeyboard");
        _builder = new DeviceMapBuilder(_testIdentity);
        _testInput1 = new TestInput(1, "A");
        _testInput2 = new TestInput(2, "B");
    }

    #endregion

    #region WithMap

    [Fact]
    public void WithMap_NullInput_ThrowsArgumentNullException()
    {
        // Arrange
        IInput? input = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithMap(input!, "Action1"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void WithMap_EmptyActionName_ThrowsArgumentNullException(string? name)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithMap(_testInput1, name!));
    }

    [Fact]
    public void WithMap_DuplicateInputId_ThrowsInvalidOperationException()
    {
        // Arrange
        var input = new TestInput(1, "A");
        _builder.WithMap(input, "Action1");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithMap(input, "Action2"));
    }

    [Fact]
    public void WithMap_DuplicateActionName_ThrowsInvalidOperationException()
    {
        // Arrange
        var input1 = new TestInput(1, "A");
        var input2 = new TestInput(2, "B");
        _builder.WithMap(input1, "Click");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithMap(input2, "Click"));
    }

    [Fact]
    public void WithMap_DuplicateActionNameCaseInsensitive_ThrowsInvalidOperationException()
    {
        // Arrange
        var input1 = new TestInput(1, "A");
        var input2 = new TestInput(2, "B");
        _builder.WithMap(input1, "Click");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.WithMap(input2, "CLICK"));
    }

    [Fact]
    public void WithMap_ValidInput_AddsToLookup()
    {
        // Arrange
        var definition = new ActionDefinition("Test", new InputAction[] { new InputAction("Click", new HashSet<InputPhase>(), _ => { }) }, isDefault: false);

        // Act
        _builder.WithMap(_testInput1, "Click");
        var result = _builder.Build(definition);

        // Assert
        Assert.Equal(_testIdentity, result.DeviceIdentity);
        Assert.Single(result.InputMaps);
    }

    [Fact]
    public void WithMap_MultipleInputs_AddsAllMappings()
    {
        // Arrange
        var definition = new ActionDefinition("Test", new InputAction[] { 
            new InputAction("Click", new HashSet<InputPhase>(), _ => { }),
            new InputAction("Move", new HashSet<InputPhase>(), _ => { })
        }, isDefault: false);

        // Act
        _builder.WithMap(_testInput1, "Click");
        _builder.WithMap(_testInput2, "Move");
        var result = _builder.Build(definition);

        // Assert
        Assert.Equal(2, result.InputMaps.Count);
    }

    #endregion

    #region Build

    [Fact]
    public void Build_NullDefinition_ThrowsArgumentNullException()
    {
        // Arrange
        ActionDefinition? definition = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _builder.Build(definition!));
    }

    [Fact]
    public void Build_ActionNotInDefinition_ThrowsInvalidOperationException()
    {
        // Arrange
        var action = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var definition = new ActionDefinition("Test", new[] { action }, isDefault: false);

        _builder.WithMap(_testInput1, "NonExistent");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _builder.Build(definition));
    }

    [Fact]
    public void Build_ValidDefinition_CreatesCorrectDeviceInputMap()
    {
        // Arrange
        var action = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var definition = new ActionDefinition("Test", new[] { action }, isDefault: false);

        _builder.WithMap(_testInput1, "Click");

        // Act
        var result = _builder.Build(definition);

        // Assert
        Assert.Equal(_testIdentity, result.DeviceIdentity);
        Assert.Single(result.InputMaps);
        Assert.Same(action, result.InputMaps.First().Action);
        Assert.Same(_testInput1, result.InputMaps.First().Input);
    }

    [Fact]
    public void Build_MultipleActions_CreatesAllMaps()
    {
        // Arrange
        var action1 = new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start }, ctx => { });
        var action2 = new InputAction("Move", new HashSet<InputPhase> { InputPhase.End }, ctx => { });
        var definition = new ActionDefinition("Test", new[] { action1, action2 }, isDefault: false);

        _builder.WithMap(_testInput1, "Click");
        _builder.WithMap(_testInput2, "Move");

        // Act
        var result = _builder.Build(definition);

        // Assert
        Assert.Equal(2, result.InputMaps.Count);
    }

    #endregion
}
