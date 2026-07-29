using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Options;

namespace OSK.Petra.Inputs.Ports;

/// <summary>
/// Handles the management of users and their devices
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IUserManager
{
    /// <summary>
    /// Creates a new user in the input syste, if configuration allows
    /// </summary>
    /// <param name="joinOptions">The options for the join operation to use</param>
    /// <returns>An output that describes if the user operation completed</returns>
    Output<IInputUser> CreateUser(UserJoinOptions joinOptions);

    /// <summary>
    /// Attempts to remove the user with the id from the manager
    /// </summary>
    /// <param name="userId">The id of the user to remove</param>
    /// <returns>true if the user existed, false if not</returns>
    bool RemoveUser(int userId);

    /// <summary>
    /// Attempts to get the user with the id
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>The user if the id is valid, otherwise null</returns>
    IInputUser? GetUser(int userId);

    /// <summary>
    /// Gets the list of users currently possessed by the manager
    /// </summary>
    /// <returns>The list of users the manager contains</returns>
    IEnumerable<IInputUser> GetUsers();

    /// <summary>
    /// Attempts to get the user that possesses a device with the given id
    /// </summary>
    /// <param name="deviceId">The id for the device</param>
    /// <returns>The user paired to the device with the id, otherwise null</returns>
    IInputUser? GetUserForDevice(int deviceId);

    /// <summary>
    /// Attempts to pair the device to the user
    /// </summary>
    /// <param name="userId">The id of the user to pair to</param>
    /// <param name="device">The device being paired</param>
    /// <returns>An output that describes if the pairing succeeded</returns>
    Output PairDevice(int userId, RuntimeDeviceIdentifier device);

    /// <summary>
    /// Attempts to remove the device from the user
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="deviceId">The device to remove</param>
    /// <returns>true if the user possessed the device, otherwise null</returns>
    bool UnpairDevice(int userId, int deviceId);

    /// <summary>
    /// Attempts to set the active definition for the user within the input system
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="definitionName">The name of the definition to set to active</param>
    /// <returns>An output that describes if the switch successded</returns>
    Output SetActiveDefinition(int userId, string definitionName);

    /// <summary>
    /// Updates and loads the input system configuration with user specified data.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item> It is important that this is run after a user saves, deletes, or performs any changes to persistent storage to ensure that the changes are reflected in the input system</item>
    /// </list>
    /// </remarks>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output that describes if hte load succeeded</returns>
    Task<Output> LoadUserConfigurationAsync(CancellationToken cancellationToken = default);
}
