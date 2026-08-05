namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IDeviceInputContext
{
    #region Variables

    int UserId { get; }

    RuntimeDeviceIdentifier DeviceIdentifier { get; }

    void SetFeature<TFeature>(TFeature feature)
        where TFeature : ICapabilityFeature;

    TFeature? GetFeature<TFeature>()
        where TFeature : ICapabilityFeature;

    #endregion
}
