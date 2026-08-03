using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public abstract class InputDeviceTopology<TEnum>: IDeviceDescriptor
    where TEnum: Enum
{
    #region Constructors

    protected InputDeviceTopology(DeviceIdentity deviceIdentity)
    {
        Identity = deviceIdentity;
        Inputs = [.. Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(GetInput)];
    }

    #endregion

    #region IInputDeviceTopology

    public DeviceIdentity Identity { get; }

    public IReadOnlyCollection<IInput> Inputs { get; }

    #endregion

    #region IDeviceTopology

    public bool Contains(IInput input)
        => Inputs.Contains(input);

    #endregion

    #region Helpers

    protected abstract IInput GetInput(TEnum value);

    #endregion
}
