using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Internal.Services;

internal class InputSystem(IInputSystemConfigurationProvider configurationProvider, IUserManager userManager,
    IInputService inputService, IInputSystemNotifier systemNotifier, IInternalSchemeService schemeService) : IInputSystem
{
    #region IInputSystem

    public InputSystemConfiguration Configuration => configurationProvider.Configuration;

    public IInputSystemNotifier Notifier => systemNotifier;

    public IUserManager UserManager => userManager;

    public ISchemeService SchemeService => schemeService;

    public bool AllowCustomSchemes => schemeService.AllowCustomSchemes;

    public bool PauseInput 
    {
        get => inputService.PauseInput;
        set => inputService.PauseInput = value;
    }

    public bool AreUserActionsSurpressed(int userId, int actionGroupId)
        => inputService.AreUserActionsSurpressed(userId, actionGroupId);

    public async Task<Output> InitializeAsync(InputSystemConfiguration configuration, CancellationToken cancellationToken = default)
    {
        configurationProvider.Configuration = configuration;

        var schemeInitializationOutput = await schemeService.LoadSchemeConfigurationAsync(cancellationToken);
        if (!schemeInitializationOutput.IsSuccessful)
        {
            return schemeInitializationOutput;
        }

        return await inputService.InitializeAsync(cancellationToken);
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
