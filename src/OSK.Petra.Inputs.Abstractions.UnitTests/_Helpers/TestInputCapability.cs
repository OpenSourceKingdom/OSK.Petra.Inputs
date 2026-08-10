using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions.UnitTests._Helpers;

public class TestableInputCapability : InputCapability<MockInput>
{
    public bool AbstractProcessCalled { get; private set; }
    public TimeSpan ReceivedDeltaTime { get; private set; }

    protected override void Process(IDeviceInputContext context, IInputState state, MockInput input, TimeSpan deltaTime)
    {
        AbstractProcessCalled = true;
        ReceivedDeltaTime = deltaTime;
    }
}
