using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Services;

namespace OSK.Petra.Inputs.Internal.Models;

internal class VirtualInputState(VirtualInputContext context, IVirtualInput virtualInput): InputState(virtualInput)
{
    #region Variables

    private readonly VirtualOriginationSource _originationSource = new(virtualInput);

    public IVirtualInput VirtualInput => virtualInput;

    #endregion

    #region InputState Overrides

    public override InputOriginationSource GetOriginationSource()
        => _originationSource;

    protected override void OnDispose()
    {
        context.RemoveState(this);
    }

    #endregion
}
