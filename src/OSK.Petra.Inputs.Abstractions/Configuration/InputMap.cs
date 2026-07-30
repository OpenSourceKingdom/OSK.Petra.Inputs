using System;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public readonly struct InputMap
{
    #region Variables

    public string ActionName { get; init; }

    public int[] InputIds { get; init; }

    #endregion

    #region Constructors

    public InputMap()
        : this([], string.Empty)
    {
    }

    public InputMap(int[] inputIds, string actionName)
    {
        InputIds = inputIds ?? throw new ArgumentNullException(nameof(inputIds));
        ActionName = actionName ?? throw new ArgumentNullException(nameof(actionName));
    }

    #endregion
}
