using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.DataStructures;
using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// A set of APIs that help to faciliate editing and saving of existing or new custom input schemes
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.UnderDevelopment)]
public interface ISchemeEditor
{
    /// <summary>
    /// Triggered when an update has been performed on the editor
    /// </summary>
    event Action<SchemeEditorUpdateTarget>? EditorUpdated;

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
    /// Gets the device registry for a specific device topology that the system
    /// supports.
    /// </summary>
    /// <param name="topologyName">
    /// The device topology to retrieve the registry for
    /// </param>
    /// <returns>
    /// The device page/registry for the specified topology, or null if the topology is not supported
    /// </returns>
    DevicePage? GetDevicePage(DeviceTopologyName topologyName);

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
    Task<Output> SaveSchemeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates input capture mode for a specific action, optionally with a timeout.
    /// </summary>
    /// <param name="action">The input action to capture input for</param>
    /// <param name="captureTimeout">The maximum time to wait for input capture, or null for no timeout</param>
    /// <returns>
    /// An output describing whether input capture was successfully initiated
    /// </returns>
    Output InitiateInputCapture(InputAction action, TimeSpan? captureTimeout = null);

    /// <summary>
    /// Aborts the current input capture operation, if one is in progress.
    /// </summary>
    void AbortInputCapture();
}
