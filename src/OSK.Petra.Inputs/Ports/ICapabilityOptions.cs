using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Ports;

public interface ICapabilityOptions<TOptions>
    where TOptions: CapabilityOptions, new()
{
    TOptions Value { get; }
}
