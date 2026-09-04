using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class InputSchemeBuilder(string name): IInputSchemeBuilder
{
    #region Variables

    private bool _default;
    private readonly Dictionary<DeviceIdentity, DeviceMapBuilder> _deviceBuilderLookup = [];
    private readonly Dictionary<string, IVirtualInput> _virtualInputs = [];

    #endregion

    #region IInputSchemeBuilder

    public string Name => name;

    public IInputSchemeBuilder MakeDefault()
    {
        _default = true;

        return this;
    }

    public IInputSchemeBuilder WithMap(DeviceIdentity deviceIdentity, long inputId, string actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            throw new ArgumentNullException(nameof(actionName));
        }

        if (!_deviceBuilderLookup.TryGetValue(deviceIdentity, out var deviceBuilder))
        {
            deviceBuilder = new DeviceMapBuilder(deviceIdentity);
            _deviceBuilderLookup[deviceIdentity] = deviceBuilder;
        }

        deviceBuilder.AddMap(inputId, actionName);
        return this;
    }

    public IInputSchemeBuilder WithVirtualInput(IVirtualInput virtualInput, string actionName)
    {
        if (virtualInput is null)
        {
            throw new ArgumentNullException(nameof(virtualInput));
        }
        if (string.IsNullOrWhiteSpace(actionName))
        {
            throw new ArgumentNullException(nameof(actionName));
        }

        if (_virtualInputs.TryGetValue(actionName, out _))
        {
            throw new InvalidOperationException($"Unable to add virtual input with action {actionName} as it was already added");
        }
        if (_virtualInputs.Values.Any(input => input.Equals(virtualInput)))
        {
            throw new InvalidOperationException($"A virtual input was attempted to be assigned to action '{actionName}' but it was already registered with a different virtual input.");
        }

        _virtualInputs.Add(actionName, virtualInput);
        return this;
    }

    #endregion

    #region Api

    public IEnumerable<DeviceIdentity> GetDeviceIdentities()
        => _deviceBuilderLookup.Keys;

    public InputScheme Build(ActionDefinition definition)
    {
        var deviceMaps = new List<DeviceInputMap>();
        foreach (var builder in _deviceBuilderLookup.Values)
        {
            deviceMaps.Add(builder.Build(definition));
        }

        var virtualMaps = new List<VirtualInputActionMap>();
        foreach (var map in _virtualInputs)
        {
            var action = definition.GetAction(map.Key);
            if (action is null)
            {
                throw new InvalidOperationException($"An input map was associated with an invalid action name, '{map.Key}', for action definition '{definition.Name}'.");
            }

            virtualMaps.Add(new(action, map.Value));
        }

        return new(definition.Name, name, deviceMaps, virtualMaps, _default, false);
    }

    #endregion
}
