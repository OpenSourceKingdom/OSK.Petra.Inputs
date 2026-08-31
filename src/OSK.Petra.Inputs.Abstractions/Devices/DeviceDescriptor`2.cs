using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public abstract class DeviceDescriptor<TEnum, TInput> : DeviceDescriptor<TInput>
    where TEnum : Enum
    where TInput : class, IInput
{
    #region Constructors

    protected DeviceDescriptor(DeviceIdentity deviceIdentity)
        : base(deviceIdentity)
    {
    }

    #endregion

    #region DeviceDescriptor Overrides

    protected override IEnumerable<TInput> GetInputs()
        => Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(GetInput);

    #endregion

    #region Helpers

    protected abstract TInput GetInput(TEnum value);

    #endregion
}
