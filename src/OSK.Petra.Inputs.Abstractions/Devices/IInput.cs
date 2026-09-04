namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// Base marker interface for all inputs.
/// </summary>
/// <remarks>
/// 💡Notes:
/// <list type="bullet">
/// <item>This should be inherited by any new input expected to be read in the input system</item>
/// <item>New inputs not baked into the library will need their <see cref="IInputCapability"/> added to the DI container in order to process it</item>
/// </list>
/// </remarks>
public interface IInput
{    
}
