using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Internal.Models;

internal class SelectedScheme: ISelectedScheme
{
    #region Variables

    private readonly HashSet<DeviceIdentity> _deviceIdentities;
    private readonly Dictionary<string, Tuple<DeviceIdentity, DeviceInputActionMap>> _configuredMaps;

    private readonly Dictionary<string, InputAction> _availableActionLookup;
    private readonly Dictionary<DeviceIdentity, Dictionary<long, IDeviceInput>> _availableInputLookup;

    internal bool InitiallyPreferred { get; }

    #endregion

    #region Constructors

    public SelectedScheme(string name, bool isReadOnly, bool isPreferred, bool isNew, IEnumerable<InputAction> availableActions, IEnumerable<DeviceMapPairing<IDeviceInput>> availableInputs, 
        IEnumerable<DeviceMapPairing<DeviceInputActionMap>> deviceMapPairings)
    {
        Name = name;
        IsNew = isNew;
        IsReadonly = isReadOnly;
        IsPreferred = isPreferred;
        InitiallyPreferred = isPreferred;

        _deviceIdentities = availableInputs is not null && availableInputs.Any() 
            ? [.. availableInputs.Select(pairing => pairing.DeviceIdentity)]
            : [];
        _availableActionLookup = availableActions.ToDictionary(action => action.Name);
        _availableInputLookup = availableInputs.ToDictionary(deviceMapPairing => deviceMapPairing.DeviceIdentity, deviceMapPairing => deviceMapPairing.Items.ToDictionary(item => item.Id));
        _configuredMaps = [];

        foreach (var deviceMapPair in deviceMapPairings.SelectMany(p => p.Items.Select(item => new { p.DeviceIdentity, Map = item })))
        {
            _configuredMaps[deviceMapPair.Map.Action.Name] = new(deviceMapPair.DeviceIdentity, deviceMapPair.Map);
        }
    }

    #endregion

    #region ISelectedScheme
    
    public bool IsNew { get; }

    public bool IsReadonly { get; }

    public string Name { get; private set; }

    public bool IsPreferred { get; private set; }

    public IReadOnlyList<DeviceMapPairing<DeviceInputActionMap>> ConfiguredInputMaps 
        => [.. _configuredMaps.Values.GroupBy(v => v.Item1).Select(inputMapGroup => new DeviceMapPairing<DeviceInputActionMap>(inputMapGroup.Key, inputMapGroup.Select(v => v.Item2)))];

    public IReadOnlyList<DeviceMapPairing<IDeviceInput>> UnpairedInputs 
        => [.. _availableInputLookup.Select(deviceInputKvp => 
        {
            var mappedDeviceInputs = _configuredMaps.Values.Where(map => map.Item1 == deviceInputKvp.Key).Select(deviceActionMapTuple => deviceActionMapTuple.Item2.InputId);
            var availableDeviceInputs = mappedDeviceInputs.Any()
                ? deviceInputKvp.Value.Values.Where(input => !mappedDeviceInputs.Contains(input.Id))
                : deviceInputKvp.Value.Values;

            return new DeviceMapPairing<IDeviceInput>(deviceInputKvp.Key, availableDeviceInputs);
        }).Where(pairing => pairing.Items.Any())];

    public IReadOnlyList<InputAction> UnpairedActions => [.. _availableActionLookup.Values.Where(action => !_configuredMaps.TryGetValue(action.Name, out _))];

    public void MakePreferred()
    {
        IsPreferred = true;
    }

    public Output SetName(string name)
    {
        if (IsReadonly)
        {
            return Out.InvalidRequest("Unable to change scheme configuration since it is read-only");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Out.InvalidRequest("Scheme name can not be empty.");
        }

        Name = name;

        return Out.Success();
    }

    public Output SetInputMap(DeviceIdentity deviceIdentity, InputAction action, IDeviceInput input)
    {
        if (!_deviceIdentities.Contains(deviceIdentity))
        {
            return Out.InvalidRequest($"Unable to set an action map for the device '{deviceIdentity}' as it not supported with the scheme.");
        }

        if (IsReadonly)
        {
            return Out.InvalidRequest("Unable to change scheme configuration since it is read-only");
        }

        if (action is null)
        {
            return Out.InvalidRequest("Action can not be empty.");
        }
        if (!_availableActionLookup.TryGetValue(action.Name, out _))
        {
            return Out.InvalidRequest($"The action '{action.Name}' is not a valid for the current scheme.");
        }

        if (input is null)
        {
            return Out.InvalidRequest("Inputs can not be empty.");
        }

        // Deassociate any current pairs with the same input or action
        if (_configuredMaps.TryGetValue(action.Name, out var currentMap))
        {
            _configuredMaps.Remove(action.Name);
        }

        var currentInputKvp = _configuredMaps.Where(kvp => kvp.Value.Item2.InputId == input.Id);
        if (currentInputKvp.Any())
        {
            var foundInput = currentInputKvp.First();
            _configuredMaps.Remove(foundInput.Value.Item2.Action.Name);
        }

        _configuredMaps[action.Name] = new(deviceIdentity, new DeviceInputActionMap(action, input.Id));

        return Out.Success();
    }

    public Output ClearConfiguredMaps()
    {
        if (IsReadonly)
        {
            return Out.InvalidRequest($"The selected scheme '{Name}' is Readonly, and thus the configured maps could not be cleared.");
        }

        _configuredMaps.Clear();
        return Out.Success();
    }

    #endregion
}
