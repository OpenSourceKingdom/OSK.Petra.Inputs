using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Internal.Models;

internal class InputUser(int id): IInputUser
{
    #region Variables

    private Dictionary<int, PairedDevice> _pairedDevices = [];

    #endregion

    #region Constructors

    internal InputUser(int id, Dictionary<int, PairedDevice> pairedDevices)
        : this(id)
    {
        _pairedDevices = pairedDevices;
    }

    #endregion

    #region IApplicationUser

    public int Id => id;

    public string ActiveDefinitionName { get; set; } = string.Empty;

    public IReadOnlyCollection<PairedDevice> PairedDevices => _pairedDevices.Values;

    public PairedDevice? GetDevice(int deviceId)
        => _pairedDevices.TryGetValue(deviceId, out var device)
            ? device
            : null;

    #endregion

    #region Helpers

    public void AddDevice(RuntimeDeviceIdentifier deviceIdentifier)
    {
        _pairedDevices[deviceIdentifier.DeviceId] = new PairedDevice(Id, deviceIdentifier);
    }

    public PairedDevice? RemoveDevice(int deviceId)
    {
        if (_pairedDevices.TryGetValue(deviceId, out var device))
        {
            _pairedDevices.Remove(deviceId);
            return device;
        }

        return null;
    }

    public IReadOnlyCollection<PairedDevice> GetPairedDevices()
        => _pairedDevices.Values;

    public PairedDevice? GetPairedDevice(int id)
        => _pairedDevices.TryGetValue(id, out var device)
            ? device
            : null;

    #endregion
}
