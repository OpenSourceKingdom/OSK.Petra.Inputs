using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;

namespace OSK.Petra.Inputs.UnitTests._Helpers;

public class TestablePointerCapability : PointerCapability
{
    public bool ProcessCalled { get; private set; }
    public TimeSpan ReceivedDeltaTime { get; private set; }

    public TestablePointerCapability() : base(Microsoft.Extensions.Options.Options.Create(new PointerCapabilityOptions())) { }

    protected override void Process(IDeviceInputContext context, IInputState state, PointerEvent pointerEvent, PointerSettings settings, TimeSpan deltaTime)
    {
        ProcessCalled = true;
        ReceivedDeltaTime = deltaTime;
    }
}
