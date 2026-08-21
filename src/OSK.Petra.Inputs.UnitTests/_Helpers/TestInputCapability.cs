using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Petra.Inputs.UnitTests._Helpers;

public class TestableInputCapability : InputCapability<PowerEvent>
{
    public bool AbstractProcessCalled { get; private set; }
    public TimeSpan ReceivedDeltaTime { get; private set; }

    protected override void Process(IDeviceInputContext context, IInputState state, PowerEvent powerEvent, TimeSpan deltaTime)
    {
        AbstractProcessCalled = true;
        ReceivedDeltaTime = deltaTime;
    }
}
