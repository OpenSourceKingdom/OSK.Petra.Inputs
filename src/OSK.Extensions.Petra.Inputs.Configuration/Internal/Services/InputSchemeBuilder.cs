using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;

internal class InputSchemeBuilder(string definitionName, string name): IInputSchemeBuilder
{
    #region Variables

    private bool _default;
    private readonly Dictionary<DeviceIdentity, DeviceInputMap> _mapLookup = [];

    #endregion

    #region IInputSchemeBuilder

    public IInputSchemeBuilder MakeDefault()
    {
        _default = true;

        return this;
    }

    public IInputSchemeBuilder WithDevice(DeviceInputMap map)
    {
        if (map is null)
        {
            throw new ArgumentNullException(nameof(map), $"A null device was configured for the input scheme '{name}' with the device toplogies '{GetDeviceNames()}' and is not usable.");
        }

        if (_mapLookup.TryGetValue(map.DeviceIdentity, out _))
        {
            throw new InvalidOperationException($"The device mapp '{map.DeviceIdentity}' already exists on the scheme '{name}' with the device topologies '{GetDeviceNames()}'.");
        }

        _mapLookup[map.DeviceIdentity] = map;

        return this;
    }

    #endregion

    #region Helpers

    private string GetDeviceNames()
        => string.Join(",", _mapLookup.Values.Select(identity => identity.DeviceIdentity.TopologyName));

    internal InputScheme Build()
        => new InputScheme(definitionName, name, _mapLookup.Values, _default, false);

    #endregion
}
