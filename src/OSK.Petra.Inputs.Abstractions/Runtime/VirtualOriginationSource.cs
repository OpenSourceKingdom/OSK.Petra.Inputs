using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

public class VirtualOriginationSource(IVirtualInput virtualInput): InputOriginationSource
{
    #region Api

    public IVirtualInput VirtualInput => virtualInput;

    #endregion
}
