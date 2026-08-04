using OSK.Petra.Inputs.Abstractions.Inputs;
using System;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IDeviceInputContext
{
    #region Variables

    int UserId { get; }

    RuntimeDeviceIdentifier DeviceIdentifier { get; }

    void SetFeature<TData>(TData data)
        where TData : ICapabilityFeature;

    #endregion
}
