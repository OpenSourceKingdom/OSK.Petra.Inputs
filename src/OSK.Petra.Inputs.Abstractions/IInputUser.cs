using System.Collections.Generic;
using OSK.Hexagonal.MetaData;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions;

/// <summary>
/// A user that interacts with the system using some form of <see cref="RuntimeDeviceIdentifier"/>
/// </summary>
///
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IInputUser
{
    /// <summary>
    /// The unique id for the user
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The currently active input definition the user is utilizing
    /// </summary>
    string ActiveInputDefinitionName { get; }

    /// <summary>
    /// The current active input scheme that is being used to interact with the input system
    /// </summary>
    ActiveInputScheme? ActiveScheme { get; }

    /// <summary>
    /// Gets the preferred input sheme that the user has for a given device combination
    /// </summary>
    /// <param name="definitionName">The name of the definition that contains the desired scheme preference</param>
    /// <param name="deviceCombinationId">The device combination of the preference being checked (i.e. xbox, playstation, etc.)</param>
    /// <returns>The preferred scheme preference for the definition and device combination, if the user has any set</returns>
    PreferredInputScheme? GetPreferredInputScheme(string definitionName, string deviceCombinationId);

    /// <summary>
    /// The current list of devices paired to the user
    /// </summary>
    IReadOnlyCollection<PairedDevice> PairedDevices { get; }

    /// <summary>
    /// Attempts to get a device from a user, provided the id
    /// </summary>
    /// <param name="deviceId">The id of the device to get</param>
    /// <returns>The device, if paired to the user, otherwise null</returns>
    PairedDevice? GetDevice(int deviceId);
}
