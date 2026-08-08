using OSK.Petra.Inputs.Abstractions.Configuration;
using System;

namespace OSK.Petra.Inputs.Exceptions;

/// <summary>
/// Ann exception that is specific to the input system validation process
/// </summary>
/// <param name="validationResult">The error validation</param>
public class InputSystemValidationException(InputConfigurationValidationResult validationResult) : Exception($"There was a validation error with the input configuration: [{validationResult.ConfigurationType}] {validationResult.Message}")
{
}
