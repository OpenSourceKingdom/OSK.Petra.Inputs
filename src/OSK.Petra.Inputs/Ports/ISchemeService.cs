using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Ports;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface ISchemeService
{
    bool AllowCustomSchemes { get; }

    /// <summary>
    /// Gets the preferred input sheme that the user has for a given input configuration
    /// </summary>
    /// <param name="userId">The user to get the preferred scheme for</param>
    /// <param name="inputConfigurationId">The input configuration id of the preference being checked (i.e. xbox, playstation, etc.)</param>
    /// <param name="definitionName">The name of the definition that contains the desired scheme preference</param>
    /// <returns>The preferred scheme preference for the definition and input configuration, if the user has any set</returns>
    PreferredInputScheme? GetPreferredInputScheme(int userId, string inputConfigurationId, string definitionName);

    InputScheme? GetActiveSchemeForUser(int userId);

    Output<InputScheme> SetActiveSchemeForDevice(int userId, DeviceIdentity deviceIdentity);

    IEnumerable<InputScheme> GetInputSchemes(string inputConfigurationId, string definitionName);

    Task<Output> SaveCustomSchemeAsync(CustomInputScheme scheme, SchemeSavePermissions savePermissions, CancellationToken cancellationToken = default);

    Task<Output> DeleteCustomSchemeAsync(string definitionName, string schemeName, CancellationToken cancellationToken = default);

    Task<Output> SavePreferredSchemeAsync(PreferredInputScheme preferredScheme, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a scheme editor that targets the provided user
    /// </summary>
    /// <param name="userId">The id of the user to target</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A scheme editor that is targeted to the user id</returns>
    Task<Output<ISchemeEditor>> GetSchemeEditorAsync(int userId, CancellationToken cancellationToken = default);
}
