using System.Threading;
using System.Threading.Tasks;
using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Models;

namespace OSK.Petra.Inputs.Ports;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IInputSchemeEditor
{
    /// <summary>
    /// The total number of definitions that a user has access to
    /// </summary>
    int TotalDefinitions { get; }

    /// <summary>
    /// The id of the current definition
    /// </summary>
    int CurrentDefinitionId { get; }

    /// <summary>
    /// The current definition
    /// </summary>
    ActionDefinition CurrentDefinition { get; }

    /// <summary>
    /// The total number of schemes a user has access to
    /// </summary>
    int TotalSchemes { get; }

    /// <summary>
    /// The id of the current schemme
    /// </summary>
    int CurrentSchemeId { get; }

    /// <summary>
    /// The total number of device combinations supported
    /// </summary>
    int TotalInputConfigurationsSupported { get; }

    /// <summary>
    /// The current device combination id
    /// </summary>
    int CurrentDeviceCombinationId { get; }

    /// <summary>
    /// The current device combination
    /// </summary>
    InputConfiguration CurrentInputConfiguration { get; }

    /// <summary>
    /// The current scheme, which will be modified if changes to the <see cref="EditableScheme"/> are committed.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This will be null if the current scheme being edited is a new custom scheme</item>
    /// </list>
    /// </remarks>
    InputScheme? CurrentScheme { get; }

    /// <summary>
    /// The current scheme being edited
    /// </summary>
    IEditableInputScheme EditableScheme { get; }

    /// <summary>
    /// Whether the scheme editor allows custom schemes
    /// </summary>
    bool AllowCustomScheme { get; }

    /// <summary>
    /// Creates a new scheme and sets it to the current
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The index should be between 0 and the <see cref="TotalInputConfigurationsSupported"/></item>
    /// <item>If null, the current device combination id will be used to create the new scheme</item>
    /// </list>
    /// </remarks>
    /// <param name="deviceCombinationId">The device combination id that the scheme is being created for</param>
    /// <returns>An output that describes the operation success</returns>
    Output CreateNewScheme(int? deviceCombinationId = null);

    /// <summary>
    /// Sets the scheme to the provided index
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The index should be between 0 and the <see cref="TotalDefinitions"/></item>
    /// </list>
    /// </remarks>
    /// <param name="definitionId">The id of the definition to edit.</param>
    void SetDefintion(int definitionId);

    /// <summary>
    /// Sets the scheme to the provided index
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The index should be between 0 and the <see cref="TotalSchemes"/></item>
    /// </list>
    /// </remarks>
    /// <param name="schemeId">The id of the scheme to edit.</param>
    void SetScheme(int schemeId);

    /// <summary>
    /// Sets the device combination to the provided index
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The index should be between 0 and the <see cref="TotalInputConfigurationsSupported"/></item>
    /// </list>
    /// </remarks>
    /// <param name="configurationId">The id of the input configuration to edit.</param>
    void SetInputConfiguration(int configurationId);

    /// <summary>
    /// Attempts to delete the current scheme
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output for the success of the operation</returns>
    Task<Output> DeleteSchemeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to save the current scheme and changes
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output for the success of the operation</returns>
    Task <Output> SaveSchemeAsync(CancellationToken cancellationToken = default);
}
