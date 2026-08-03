using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Exceptions;
using OSK.Petra.Inputs.Notifications;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystem(IInputSystemConfigurationProvider configurationProvider, IUserManager userManager,
    IInputService inputService, IInputSystemNotifier systemNotifier, SchemeService schemeService) : IInputSystem
{
    #region Variables

    private bool _isPaused;

    #endregion

    #region IInputSystem

    public InputSystemConfiguration Configuration => configurationProvider.Configuration;

    public IInputSystemNotifier Notifier => systemNotifier;

    public IUserManager UserManager => userManager;

    public ISchemeService SchemeService => schemeService;

    public bool AllowCustomSchemes => schemeService.AllowCustomSchemes;

    public bool PauseInput 
    {
        get => _isPaused;
        set
        {
            if (_isPaused == PauseInput)
            {
                return;
            }

            _isPaused = value;
            inputService.PauseInput = value;
            systemNotifier.Notify(new InputMonitorStatusChangedNotification(!value));
        }
    }

    public async Task<Output> InitializeAsync(InputSystemConfiguration configuration, CancellationToken cancellationToken = default)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var validationResult = InputSystemConfigurationValidator.ValidateConfiguration(configuration);
        if (!validationResult.IsValid)
        {
            throw new InputSystemValidationException($"The provided input configuration was invalid. Message: {validationResult}");
        }

        configurationProvider.Configuration = configuration;
        return await schemeService.LoadSchemeConfigurationAsync(cancellationToken);
    }

    public void Update(TimeSpan deltaTime)
    {
        if (PauseInput)
        {
            return;
        }

        inputService.Update(deltaTime);
    }

    #endregion
}
