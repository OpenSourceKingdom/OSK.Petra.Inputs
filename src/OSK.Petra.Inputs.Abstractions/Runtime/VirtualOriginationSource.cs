using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// Describes the origination source for a particular input within an <see cref="IInputEventContext"/> that originated from a virtual input
/// </summary>
/// <param name="virtualInput">The input that triggered the event</param>
public class VirtualOriginationSource(IVirtualInput virtualInput): InputOriginationSource
{
    #region Api

    /// <summary>
    /// The input that triggered the event
    /// </summary>
    public IVirtualInput VirtualInput => virtualInput;

    #endregion
}
