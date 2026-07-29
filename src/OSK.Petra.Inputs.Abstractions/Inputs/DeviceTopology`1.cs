using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public abstract class InputDeviceTopology<TEnum>: IDeviceTopology
    where TEnum: Enum
{
    #region Variables

    private readonly Dictionary<int, IInput> _inputLookup;

    #endregion

    #region Constructors

    protected InputDeviceTopology(DeviceTopologyName deviceType)
    {
        Name = deviceType;
        _inputLookup = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(GetInput).ToDictionary(input => input.Id);
    }

    #endregion

    #region IInputDeviceTopology

    public DeviceTopologyName Name { get; }

    public IReadOnlyCollection<IInput> Inputs => _inputLookup.Values;

    public bool TryGetInput(int inputId, [NotNullWhen(true)] out IInput? input)
    {
        input = _inputLookup.TryGetValue(inputId, out var deviceInput)
            ? deviceInput
            : null;
        return input is not null;
    }

    #endregion

    #region Helpers

    protected abstract IInput GetInput(TEnum value);

    #endregion
}
