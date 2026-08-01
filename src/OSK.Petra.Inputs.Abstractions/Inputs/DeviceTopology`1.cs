using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Inputs;

public abstract class InputDeviceTopology<TEnum>: IDeviceTopology
    where TEnum: Enum
{
    #region Constructors

    protected InputDeviceTopology(DeviceTopologyName deviceType)
    {
        Name = deviceType;
        Inputs = [.. Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(GetInput)];
    }

    #endregion

    #region IInputDeviceTopology

    public DeviceTopologyName Name { get; }

    public IReadOnlyCollection<IInput> Inputs { get; }

    #endregion

    #region Helpers

    protected abstract IInput GetInput(TEnum value);

    #endregion
}
