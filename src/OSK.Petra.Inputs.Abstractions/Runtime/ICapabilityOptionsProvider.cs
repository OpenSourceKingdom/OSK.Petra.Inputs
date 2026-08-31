namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface ICapabilityOptionsProvider
{
    TCapabilityOptions Get<TCapabilityOptions>()
        where TCapabilityOptions: CapabilityOptions, new();
}
