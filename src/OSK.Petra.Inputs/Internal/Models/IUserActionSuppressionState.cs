using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Internal.Models;

internal interface IUserActionSuppressionState
{
    void Suppress(int[]? actionsGroups, int[]? users);

    void Enable(int[]? actionsGroups, int[]? users);

    bool IsSuppressed(int userId, int actionGroup);
}
