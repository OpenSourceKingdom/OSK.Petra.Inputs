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
    string ActiveDefinitionName { get; }

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
