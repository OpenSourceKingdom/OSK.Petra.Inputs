using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public abstract class DeviceDescriptor<TInput>: IDeviceDescriptor
    where TInput: class, IInput
{
    #region Variables

    private readonly Dictionary<int, TInput> _inputLookup;

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

    IReadOnlyCollection<IInput> IDeviceDescriptor.Inputs => Inputs;

    public IInput? GetInput(int id)
        => _inputLookup.TryGetValue(id, out var input) ? input : null;

    #endregion

    #region Api

    public IReadOnlyList<TInput> Inputs => [.. _inputLookup.Values];

    #endregion

    #region Helpers

    protected abstract IEnumerable<TInput> GetInputs();

    #endregion
}
