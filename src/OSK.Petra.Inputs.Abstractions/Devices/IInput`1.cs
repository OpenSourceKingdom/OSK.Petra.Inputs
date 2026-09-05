namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// An input that contains specific settings, which a user might be able to adjust
/// </summary>
/// <typeparam name="TSettings">The type of settings the input utilizes</typeparam>
public interface IInput<TSettings>: IInput
    where TSettings: IInputSettings
{
    /// <summary>
    /// The settings that the input utilizes in conjunction with an <see cref="IInputCapability"/>
    /// </summary>
    TSettings Settings { get; }
}
