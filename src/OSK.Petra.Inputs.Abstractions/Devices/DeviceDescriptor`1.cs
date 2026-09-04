using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public abstract class DeviceDescriptor<TInput>: IDeviceDescriptor
    where TInput: class, IDeviceInput
{
    #region Variables

    private readonly Dictionary<long, TInput> _inputLookup;

    #endregion

    #region Constructors

    protected DeviceDescriptor(DeviceIdentity deviceIdentity)
    {
        Identity = deviceIdentity;
        var inputs = GetInputs() ?? throw new ArgumentNullException($"The loaded inputs were null for device {deviceIdentity}");
        _inputLookup = inputs.ToDictionary(input => input.Id);
    }

    #endregion

    #region IDeviceDescriptor

    public DeviceIdentity Identity { get; }

    IReadOnlyCollection<IDeviceInput> IDeviceDescriptor.Inputs => Inputs;

    public IDeviceInput? GetInput(long id)
        => _inputLookup.TryGetValue(id, out var input) ? input : null;

    #endregion

    #region Api

    public IReadOnlyList<TInput> Inputs => [.. _inputLookup.Values];

    #endregion

    #region Helpers

    protected abstract IEnumerable<TInput> GetInputs();

    #endregion
}
