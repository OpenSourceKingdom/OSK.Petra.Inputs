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

    private readonly Dictionary<string, InputActionPair> _inputMaps;
    private readonly Dictionary<string, InputAction> _availableActionLookup;
    private readonly Dictionary<int, IInput> _availableInputLookup;

    internal bool InitiallyPreferred { get; }

    #endregion

    #region Constructors

    public SelectedScheme(string name, bool isReadOnly, bool isPreferred, IEnumerable<InputAction> availableActions, IEnumerable<IInput> availableInputs, IEnumerable<InputActionMap> maps)
    {
        Name = name;
        IsReadonly = isReadOnly;
        IsPreferred = isPreferred;
        InitiallyPreferred = isPreferred;

        var allInputLookup = availableInputs.ToDictionary(input => input.Id);
        var actionLookup = availableActions.ToDictionary(action => action.Name);

        _availableActionLookup = availableActions.ToDictionary(action => action.Name);
        _availableInputLookup = availableInputs.ToDictionary(input => input.Id);

        _inputMaps = []; 
        foreach (var map in maps)
        {
            _inputMaps[map.ActionName] = new InputActionPair()
            {
                Action = _availableActionLookup[map.ActionName],
                Input = map.Input
            };

            _availableActionLookup.Remove(map.ActionName);
            _availableInputLookup.Remove(map.Input.Id);
        }
    }

    #endregion

    #region ISelectedScheme

    public bool IsReadonly { get; }

    public string Name { get; private set; }

    public bool IsPreferred { get; private set; }

    public IReadOnlyCollection<InputActionPair> InputMaps => [.. _inputMaps.Values];

    public IReadOnlyCollection<InputAction> UnpairedActions => [.. _availableActionLookup.Values];

    public IReadOnlyCollection<IInput> UnpairedInputs => [.. _availableInputLookup.Values];

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

    public Output SetInputMap(InputAction action, IInput input)
    {
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
            _availableInputLookup[currentMap.Input.Id] = currentMap.Input;
            _inputMaps.Remove(action.Name);
        }

        var currentInputKvp = _inputMaps.Where(kvp => kvp.Value.Input.Id == input.Id);
        if (currentInputKvp.Any())
        {
            var foundInput = currentInputKvp.First();
            _availableActionLookup[foundInput.Value.Action.Name] = foundInput.Value.Action;
            _inputMaps.Remove(foundInput.Value.Action.Name);
        }

        _availableActionLookup.Remove(action.Name);
        _inputMaps[action.Name] = new InputActionPair()
        {
            Input = input,
            Action = action
        };

        return Out.Success();
    }

    #endregion
}
