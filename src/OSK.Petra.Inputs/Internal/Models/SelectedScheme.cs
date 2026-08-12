using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Models;

namespace OSK.Petra.Inputs.Internal.Models;

internal class SelectedScheme: ISelectedScheme
{
    #region Variables

    private readonly HashSet<DeviceIdentity> _deviceIdentities;
    private readonly Dictionary<string, Tuple<DeviceIdentity, InputActionMap>> _inputMaps;
    private readonly Dictionary<string, InputAction> _availableActionLookup;
    private readonly Dictionary<DeviceIdentity, Dictionary<int, IInput>> _availableInputLookup;

    internal bool InitiallyPreferred { get; }

    #endregion

    #region Constructors

    public SelectedScheme(string name, bool isReadOnly, bool isPreferred, bool isNew, IEnumerable<InputAction> availableActions, IEnumerable<DeviceMapPairing<IInput>> availableInputs, 
        IEnumerable<DeviceMapPairing<InputActionMap>> deviceMapPairings)
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
        _inputMaps = [];

        foreach (var deviceMapPair in deviceMapPairings.SelectMany(p => p.Items.Select(item => new { p.DeviceIdentity, Map = item })))
        {
            _inputMaps[deviceMapPair.Map.Action.Name] = new(deviceMapPair.DeviceIdentity, deviceMapPair.Map);

            _availableActionLookup.Remove(deviceMapPair.Map.Action.Name);

            if (_availableInputLookup.TryGetValue(deviceMapPair.DeviceIdentity, out var deviceInputLookup))
            {
                deviceInputLookup.Remove(deviceMapPair.Map.Input.Id);
                if (deviceInputLookup.Count is 0)
                {
                    _availableInputLookup.Remove(deviceMapPair.DeviceIdentity);
                }
            }
        }
    }

    #endregion

    #region ISelectedScheme
    
    public bool IsNew { get; }

    public bool IsReadonly { get; }

    public string Name { get; private set; }

    public bool IsPreferred { get; private set; }

    public IReadOnlyList<DeviceMapPairing<InputActionMap>> ConfiguredInputMaps => [.. _inputMaps.Values.GroupBy(v => v.Item1).Select(inputMapGroup => new DeviceMapPairing<InputActionMap>(inputMapGroup.Key, inputMapGroup.Select(v => v.Item2)))];

    public IReadOnlyList<DeviceMapPairing<IInput>> UnpairedInputs => [.. _availableInputLookup.Select(kvp => new DeviceMapPairing<IInput>(kvp.Key, kvp.Value.Values))];

    public IReadOnlyList<InputAction> UnpairedActions => [.. _availableActionLookup.Values];

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

    public Output SetInputMap(DeviceIdentity deviceIdentity, InputAction action, IInput input)
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
        if (input is null)
        {
            return Out.InvalidRequest("Inputs can not be empty.");
        }

        // Deassociate any current pairs with the same input or action
        if (_inputMaps.TryGetValue(action.Name, out var currentMap))
        {
            if (!_availableInputLookup.TryGetValue(currentMap.Item1, out var lookup))
            {
                _availableInputLookup[currentMap.Item1] = [];
            }

            _availableInputLookup[currentMap.Item1][currentMap.Item2.Input.Id] = currentMap.Item2.Input;
            _inputMaps.Remove(action.Name);
        }

        var currentInputKvp = _inputMaps.Where(kvp => kvp.Value.Item2.Input.Id == input.Id);
        if (currentInputKvp.Any())
        {
            var foundInput = currentInputKvp.First();
            _availableActionLookup[foundInput.Value.Item2.Action.Name] = foundInput.Value.Item2.Action;
            _inputMaps.Remove(foundInput.Value.Item2.Action.Name);
        }

        _availableActionLookup.Remove(action.Name);

        if (_availableInputLookup.TryGetValue(deviceIdentity, out var deviceLookup))
        {
            deviceLookup.Remove(input.Id);
            if (deviceLookup.Count is 0)
            {
                _availableInputLookup.Remove(deviceIdentity);
            }
        }

        _inputMaps[action.Name] = new(deviceIdentity, new InputActionMap(action, input));

        return Out.Success();
    }

    #endregion
}
