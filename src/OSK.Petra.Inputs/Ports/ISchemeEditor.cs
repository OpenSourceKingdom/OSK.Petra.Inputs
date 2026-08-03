using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.DataStructures;
using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;

namespace OSK.Petra.Inputs.Ports;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface ISchemeEditor
{
    /// <summary>
    /// Triggered when an update has been performed on the editor
    /// </summary>
    event Action? EditorUpdated;

    /// <summary>
    /// A navigatable collection of input configuration
    /// </summary>
    ICollectionNavigator<InputConfiguration> InputConfigurationNavigator { get; }

    /// <summary>
    /// A navigatable collection of action definitions
    /// </summary>
    ICollectionNavigator<ActionDefinition> DefinitionNavigator { get; }

    /// <summary>
    /// A navigatable collection of schemes
    /// </summary>
    ICollectionNavigator<InputScheme> SchemeNavigator { get; }

    /// <summary>
    /// The current scheme being viewed and/or edited
    /// </summary>
    ISelectedScheme SelectedScheme { get; }

    /// <summary>
    /// Whether the scheme editor allows custom schemes
    /// </summary>
    bool AllowCustomScheme { get; }

    /// <summary>
    /// Gets a registry of known devices that the system supports
    /// </summary>
    /// <returns>Theregistry for the device topology</returns>
    DeviceCatalog GetDeviceRegistry(DeviceTopologyName topologyName);

    /// <summary>
    /// Sets the editor's device for the given topology that is used to edit the <see cref="SelectedScheme"/>
    /// </summary>
    /// <param name="topologyName">The device topology</param>
    /// <param name="deviceName">The device name in the topology</param>
    /// <returns>An output that describes the operation success</returns>
    Output SetSchemeDevice(DeviceTopologyName topologyName, string deviceName);

    /// <summary>
    /// Creates a new scheme and sets it to the current
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The index should be between 0 and the total number of configurations in <see cref="InputConfigurationNavigator"/></item>
    /// <item>If null, the current input configuration id will be used to create the new scheme</item>
    /// </list>
    /// </remarks>
    /// <param name="inputConfigurationid">The input configuration id that the scheme is being created for</param>
    /// <returns>An output that describes the operation success</returns>
    Output CreateNewScheme(int? inputConfigurationid = null);

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
