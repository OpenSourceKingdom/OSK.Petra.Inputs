using OSK.Petra.Inputs.Abstractions.Configuration;
using System;

namespace OSK.Petra.Inputs.Exceptions;

/// <summary>
/// Exception thrown when the input system configuration fails validation.
/// </summary>
public class InputSystemValidationException(InputConfigurationValidationResult validationResult) 
    : Exception($"There was a validation error with the input configuration: [{validationResult.ConfigurationType}] {validationResult.Message}")
{
}
