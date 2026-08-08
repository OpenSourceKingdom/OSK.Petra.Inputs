using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

public interface IDeviceMapBuilder<TInput>
    where TInput: IInput
{
    IDeviceMapBuilder<TInput> WithMap(TInput input, string actionName);
}