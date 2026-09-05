using OSK.Petra.Inputs.Abstractions;

namespace OSK.Petra.Inputs.Internal.Models;

public readonly struct DevicePairingDetails(IInputUser user, bool missingDevice, int totalPairedDevices, int totalCompletedConfigurations, int minimumDevicesToCompleteCombination)
{
    #region Variables

    public IInputUser User => user;

    public bool MissingDevice => missingDevice;

    public int TotalPairedDevices => totalPairedDevices;

    public int TotalCompletedConfigurations => totalCompletedConfigurations;

    public int MinimumDevicesToCompleteCombination => minimumDevicesToCompleteCombination;

    #endregion
}
