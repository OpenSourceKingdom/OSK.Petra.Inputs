using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// The core input system that consumers will use when wanting to manage and handle input
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerPointOfEntry)]
public interface IInputSystem
{
    /// <summary>
    /// The current input system configuration used by the system
    /// </summary>
    InputSystemConfiguration Configuration { get; }

    /// <summary>
    /// The notifier that transmits notifications events
    /// </summary>
    IInputSystemNotifier Notifier { get; }

    /// <summary>
    /// Handles scheme management related APIs
    /// </summary>
    ISchemeService SchemeService { get; }

    /// <summary>
    /// The user manager the input system is using
    /// </summary>
    IUserManager UserManager { get; }

    /// <summary>
    /// Describes if the input system is capable of handling custom input schemes or not
    /// </summary>
    bool AllowCustomSchemes { get; }

    /// <summary>
    /// Pauses or resumes input processing
    /// </summary>
    bool PauseInput { get; set; }

    /// <summary>
    /// Initializes the input system
    /// </summary>
    /// <param name="configuration">The configuration the input system will use. It must be valid or an exception could be thrown</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output that describes whether the configuration was fully initialized with the input system</returns>
    Task<Output> InitializeAsync(InputSystemConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the input system using the specified delta time.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This process method may be ignored by the input system if the input system is pausing input</item>
    /// </list>
    /// </remarks>
    /// <param name="deltaTime">
    /// The elapsed time since the last update, used for input processing
    /// calculations
    /// </param>
    void Update(TimeSpan deltaTime);

    /// <summary>
    /// Determines whether user actions in a specific action group are currently being suppressed.
    /// </summary>
    /// <param name="userId">The user ID to check suppression status for</param>
    /// <param name="actionGroupId">The action group ID to check for suppression</param>
    /// <returns>
    /// True if actions in the specified group are suppressed for the user; otherwise false
    /// </returns>
    bool AreUserActionsSurpressed(int userId, int actionGroupId);
}
