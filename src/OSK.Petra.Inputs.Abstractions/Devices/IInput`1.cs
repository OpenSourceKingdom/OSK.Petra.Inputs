namespace OSK.Petra.Inputs.Abstractions.Devices;

public interface IInput<TSettings>: IInput
    where TSettings: IInputSettings
{
    TSettings Settings { get; }
}
