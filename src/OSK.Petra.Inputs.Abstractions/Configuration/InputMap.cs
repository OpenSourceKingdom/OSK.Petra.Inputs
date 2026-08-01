using System;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public readonly struct InputMap
{
    #region Variables

    public string ActionName { get; init; }

    public int InputId { get; init; }

    #endregion

    #region Constructors

    public InputMap()
        : this(0, string.Empty)
    {
    }

    public InputMap(int inputId, string actionName)
    {
        InputId = inputId;
        ActionName = actionName ?? throw new ArgumentNullException(nameof(actionName));
    }

    #endregion
}
