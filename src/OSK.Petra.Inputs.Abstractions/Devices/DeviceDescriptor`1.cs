using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// A device descriptor that utilizes a specific T input
/// </summary>
/// <typeparam name="TInput">A strongly typed input the device will use</typeparam>
public abstract class DeviceDescriptor<TInput>: IDeviceDescriptor
    where TInput: class, IDeviceInput
{
    #region Variables

    private readonly Dictionary<long, TInput> _inputLookup;

    #endregion

    #region Constructors

    protected DeviceDescriptor(DeviceIdentity deviceIdentity, IEnumerable<TInput> inputs)
    {
        Identity = deviceIdentity;
        _inputLookup = inputs?.ToDictionary(input => input.Id) ?? throw new ArgumentNullException($"The loaded inputs were null for device {deviceIdentity}");
    }

    #endregion

    #region IDeviceDescriptor

    /// <inheritdoc/>
    public DeviceIdentity Identity { get; }

    /// <inheritdoc/>
    IReadOnlyCollection<IDeviceInput> IDeviceDescriptor.Inputs => Inputs;

    /// <inheritdoc/>
    public IDeviceInput? GetInput(long id)
        => _inputLookup.TryGetValue(id, out var input) ? input : null;

    #endregion

    #region Api

    /// <summary>
    /// The input collection of the specific strongly typed inputs
    /// </summary>
    public IReadOnlyList<TInput> Inputs => [.. _inputLookup.Values];

    #endregion
}
