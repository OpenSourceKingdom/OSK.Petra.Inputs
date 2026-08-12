using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Notifications;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystem(IInputSystemConfigurationProvider configurationProvider, IUserManager userManager,
    IInputService inputService, IInputSystemNotifier systemNotifier, IInternalSchemeService schemeService) : IInputSystem
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
            if (_isPaused == value)
            {
                return;
            }

            _isPaused = value;
            inputService.PauseInput = value;
            systemNotifier.Notify(new InputMonitorStatusChangedNotification(!value));
        }
    }

    public Task<Output> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var validationResult = InputSystemConfigurationValidator.ValidateConfiguration(configurationProvider.Configuration);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"The provided input configuration was invalid. Message: {validationResult}");
        }

        return schemeService.LoadSchemeConfigurationAsync(cancellationToken);
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
