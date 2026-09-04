using OSK.Petra.Inputs.Abstractions.Devices;
using System;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public readonly struct VirtualInputActionMap
{
    #region Variables

    /// <summary>
    /// The action name this maps to
    /// </summary>
    public InputAction Action { get; init; }

    /// <summary>
    /// The virtual input this maps to
    /// </summary>
    public IVirtualInput VirtualInput { get; init; }

    #endregion

    #region Constructors

    public VirtualInputActionMap(InputAction action, IVirtualInput input)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        VirtualInput = input;
    }

    #endregion
}
