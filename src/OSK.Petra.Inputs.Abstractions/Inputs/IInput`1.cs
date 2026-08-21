namespace OSK.Petra.Inputs.Abstractions.Inputs;

public interface IInput<TSettings>: IInput
    where TSettings: IInputSettings
{
    TSettings Settings { get; }
}
