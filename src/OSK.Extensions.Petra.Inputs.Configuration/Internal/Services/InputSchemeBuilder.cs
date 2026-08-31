using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class InputSchemeBuilder(string name): IInputSchemeBuilder
{
    #region Variables

    private bool _default;
    private readonly Dictionary<DeviceIdentity, DeviceMapBuilder> _deviceBuilderLookup = [];

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

    public InputScheme Build(ActionDefinition definition)
    {
        var deviceMaps = new List<DeviceInputMap>();
        foreach (var builder in _deviceBuilderLookup.Values)
        {
            deviceMaps.Add(builder.Build(definition));
        }

        return new(definition.Name, name, deviceMaps, _default, false);
    }

    #endregion

    #region Api

    public IEnumerable<DeviceIdentity> GetDeviceIdentities()
        => _deviceBuilderLookup.Keys;

    #endregion
}
