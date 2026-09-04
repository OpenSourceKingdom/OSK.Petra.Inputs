using OSK.Petra.Inputs.Abstractions.Devices;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IUserInputContext
{
    #region Variables

    int UserId { get; }

    IVirtualInputContext VirtualInputContext { get; }

    bool TryGetInputState(DeviceIdentity identity, long inputId, [NotNullWhen(true)] out IInputState? state);

    void SetFeature<TFeature>(TFeature feature)
        where TFeature : ICapabilityFeature;

    TFeature? GetFeature<TFeature>()
        where TFeature : ICapabilityFeature;

    #endregion
}
