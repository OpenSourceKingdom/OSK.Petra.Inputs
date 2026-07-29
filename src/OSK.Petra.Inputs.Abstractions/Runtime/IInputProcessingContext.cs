using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public interface IInputProcessingContext
{
    #region Variables

    RuntimeDeviceIdentifier DeviceIdentifier { get; }

    IInput Input { get; }

    void SetFeature<TData>(TData data)
        where TData : CapabilityData;

    #endregion
}
