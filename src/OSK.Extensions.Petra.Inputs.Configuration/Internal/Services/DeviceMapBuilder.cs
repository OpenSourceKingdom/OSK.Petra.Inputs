using OSK.Petra.Inputs.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class DeviceMapBuilder(DeviceIdentity identity)
{
    #region Variables

    private readonly Dictionary<long, string> _maps = [];

    #endregion

    #region IDeviceMapBuilder

    public DeviceIdentity DeviceIdentity => identity;

    public DeviceInputMap Build(ActionDefinition definition)
    {
        if (definition is null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        List<DeviceInputActionMap> actionMaps = [];
        foreach (var kvp in _maps)
        {
            var action = definition.GetAction(kvp.Value);
            if (action is null)
            {
                throw new InvalidOperationException($"There is a map for device '{identity}' that has an action name '{kvp.Value}' for definition '{definition.Name}', which does not match any known action.");
            }

            actionMaps.Add(new(action, kvp.Key));
        }

        return new()
        {
            DeviceIdentity = identity,
            InputMaps = actionMaps
        };
    }

    public void AddMap(long inputId, string actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            throw new ArgumentNullException(nameof(actionName));
        }

        if (_maps.TryGetValue(inputId, out _))
        {
            throw new InvalidOperationException($"Unable to add device action map for input '{inputId}' on device '{identity}' for action '{actionName}' since the input has already been mapped.");
        }
        if (_maps.Values.Any(map => map.Equals(actionName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Unable to add device action map for input '{inputId}' on device '{identity}' for action '{actionName}' since the action has already been mapped.");
        }

        _maps[inputId] = actionName;
    }

    #endregion
}
