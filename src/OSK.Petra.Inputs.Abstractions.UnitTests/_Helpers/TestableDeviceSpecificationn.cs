using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

public class TestableDeviceSpecification : DeviceSpecification
{
    public int LookupPopulateCount { get; private set; }

    private readonly Dictionary<int, IInput> _testInputs = new()
        {
            { 1, new MockInput(1) },
            { 2, new MockInput(2) }
        };

    public override DeviceIdentity DeviceIdentity => default;

    public override IReadOnlyCollection<IInput> GetInputs()
    {
        LookupPopulateCount++;
        return _testInputs.Values.ToList();
    }

    public new bool TryGetInput(int inputId, out IInput input)
    {
        // Call base to trigger lazy initialization
        var result = base.TryGetInput(inputId, out input);

        return result;
    }
}