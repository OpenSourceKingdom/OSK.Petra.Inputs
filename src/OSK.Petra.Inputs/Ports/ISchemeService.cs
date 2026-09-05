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

/// <summary>
/// The APIs that manage input schemes in the input system
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface ISchemeService
{
    /// <summary>
    /// Whether custom schemes can be created and used by the input system.
    /// </summary>
    bool AllowCustomSchemes { get; }

    /// <summary>
    /// Gets the preferred input sheme that the user has for a given input configuration
    /// </summary>
    /// <param name="userId">The user to get the preferred scheme for</param>
    /// <param name="inputConfigurationId">The input configuration id of the preference being checked (i.e. xbox, playstation, etc.)</param>
    /// <param name="definitionName">The name of the definition that contains the desired scheme preference</param>
    /// <returns>The preferred scheme preference for the definition and input configuration, if the user has any set</returns>
    PreferredInputScheme? GetPreferredInputScheme(int userId, string inputConfigurationId, string definitionName);

    /// <summary>
    /// Gets the active input scheme currently assigned to the user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve the scheme for</param>
    /// <returns>The active input scheme for the user, or null if no scheme is set</returns>
    InputScheme? GetActiveSchemeForUser(int userId);

    /// <summary>
    /// Retrieves input glyphs (visual representations) for a user action.
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="actionName">The name of the action to get glyphs for</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A collection of input glyphs that map to the action on the current device</returns>
    Task<IEnumerable<InputGlyph>> GetGlyphsForUserActionAsync(int userId, string actionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Informs the scheme service to set the active input scheme for a user based on the provided device.
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="deviceIdentity">The identity of the device to set a scheme for</param>
    /// <returns>
    /// An output containing the newly activated scheme if the operation succeeded
    /// </returns>
    Output<InputScheme> SetActiveSchemeForDevice(int userId, DeviceIdentity deviceIdentity);

    /// <summary>
    /// Gets all input schemes for a specific input configuration and action definition.
    /// </summary>
    /// <param name="inputConfigurationId"> The input configuration ID</param>
    /// <param name="definitionName">The name of the action definition</param>
    /// <returns>An enumerable of input schemes matching the specified configuration and definition</returns>
    IEnumerable<InputScheme> GetInputSchemes(string inputConfigurationId, string definitionName);

    /// <summary>
    /// Saves a custom input scheme to storage.
    /// </summary>
    /// <param name="scheme">The custom input scheme to save</param>
    /// <param name="savePermissions">Permissions controlling how the scheme can be saved</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output describing whether the scheme was successfully saved</returns>
    Task<Output> SaveCustomSchemeAsync(CustomInputScheme scheme, SchemeSavePermissions savePermissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a custom input scheme from storage.
    /// </summary>
    /// <param name="definitionName">The name of the definition containing the scheme</param>
    /// <param name="schemeName">The name of the scheme to delete</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output describing whether the scheme was successfully deleted</returns>
    Task<Output> DeleteCustomSchemeAsync(string definitionName, string schemeName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a preferred input scheme for a user.
    /// </summary>
    /// <param name="preferredScheme">The preferred input scheme to save for the user</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output describing whether the preference was successfully saved</returns>
    Task<Output> SavePreferredSchemeAsync(PreferredInputScheme preferredScheme, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a scheme editor that targets the provided user
    /// </summary>
    /// <param name="userId">The id of the user to target</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A scheme editor that is targeted to the user id</returns>
    Task<Output<ISchemeEditor>> GetSchemeEditorAsync(int userId, CancellationToken cancellationToken = default);
}
