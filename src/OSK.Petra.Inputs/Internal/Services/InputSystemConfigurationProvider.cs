using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Exceptions;
using System;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystemConfigurationProvider : IInputSystemConfigurationProvider
{
    #region Variables

    private InputSystemConfiguration? _configuration;

    #endregion

    #region IInputSystemConfigurationProvider

    public InputSystemConfiguration Configuration 
    {
        get => _configuration ?? throw new InvalidOperationException("The input system configuration has not been initialized");
        set
        {
            var validationResult = InputSystemConfigurationValidator.ValidateConfiguration(value);
            if (!validationResult.IsValid)
            {
                throw new InputSystemValidationException(validationResult);
            }

            _configuration = value;
        }
    }

    #endregion
}
