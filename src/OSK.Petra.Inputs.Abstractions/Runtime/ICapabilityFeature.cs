namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// A feature is a broader set of information beyond a single input. For example, when an <see cref="IInputEventContext"/> is triggered with an action, it contains
/// the specific <see cref="ICapabilityDetails"/> that were processed for that particular input, but a feature is used to get information about all the devices or input data
/// a particular user has. So, you have input details for a button being pressed, but you want information from an input sensor (accelerometer, etc.) or pointer, you'd
/// request the capability feature.
/// 
/// It is expected that the features being used are being applied by the capability
/// </summary>
public interface ICapabilityFeature
{
}
