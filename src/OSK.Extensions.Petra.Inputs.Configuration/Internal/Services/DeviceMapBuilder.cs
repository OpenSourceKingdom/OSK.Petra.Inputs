using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class DeviceMapBuilder(DeviceIdentity identity)
{
    #region Variables

    private readonly Dictionary<int, (string ActionName, IInput Input)> _maps = [];

    #endregion

    #region IDeviceMapBuilder

    public DeviceIdentity DeviceIdentity => identity;

    public DeviceInputMap Build(ActionDefinition definition)
    {
        if (definition is null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        List<InputActionMap> actionMaps = [];
        foreach (var map in _maps.Values)
        {
            var action = definition.GetAction(map.ActionName);
            if (action is null)
            {
                throw new InvalidOperationException($"There is a map for device '{identity}' that has an action name '{map.ActionName}' for definition '{definition.Name}', which does not match any known action.");
            }

            actionMaps.Add(new(action, map.Input));
        }

        return new()
        {
            DeviceIdentity = identity,
            InputMaps = actionMaps
        };
    }

    public void AddMap(IInput input, string actionName)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        if (string.IsNullOrWhiteSpace(actionName))
        {
            throw new ArgumentNullException(nameof(actionName));
        }

        if (_maps.TryGetValue(input.Id, out _))
        {
            throw new InvalidOperationException($"Unable to add device action map for input '{input.GetGlyph().Symbol}' on device '{identity}' for action '{actionName}' since the input has already been mapped.");
        }
        if (_maps.Values.Any(map => map.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Unable to add device action map for input '{input.GetGlyph().Symbol}' on device '{identity}' for action '{actionName}' since the action has already been mapped.");
        }

        _maps[input.Id] = new(actionName, input);
    }

    #endregion
}
