using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSK.Petra.Inputs.Internal.Models;

internal class UserActionSuppressionState: IUserActionSuppressionState
{
    #region Variables

    private bool _globalActionSuppression;
    private readonly Dictionary<int, bool> _userGlobalSuppressionLookup = [];
    private readonly Dictionary<int, HashSet<int>> _actionGroupSuppressionLookup = [];

    #endregion

    #region IUserActionSuppressionState

    public void Suppress(int[]? actionsGroups, int[]? users)
    {
        // Already suppressed
        if (_globalActionSuppression)
        {
            return;
        }

        var hasActionFilter = actionsGroups is not null && actionsGroups.Length > 0;
        var userFilter = users is null || users.Length is 0
            ? []
            : users.Where(user => !_userGlobalSuppressionLookup.TryGetValue(user, out var isSuppressed) || !isSuppressed);
        var hasUsers = userFilter.Any();

        // Global suppression
        if (!hasUsers && !hasActionFilter)
        {
            _globalActionSuppression = true;
            _actionGroupSuppressionLookup.Clear();
            _userGlobalSuppressionLookup.Clear();
            return;
        }

        // Global user action suppression
        if (hasUsers && !hasActionFilter)
        {
            foreach (var userId in users!)
            {
                _userGlobalSuppressionLookup[userId] = true;
                foreach (var userActionGroupSupression in _actionGroupSuppressionLookup.Values)
                {
                    userActionGroupSupression.Remove(userId);
                }
            }

            return;
        }

        // Global action group suppression
        foreach (var actionGroup in actionsGroups!)
        {
            if (!_actionGroupSuppressionLookup.TryGetValue(actionGroup, out var suppressedUsers))
            {
                suppressedUsers = [];
                _actionGroupSuppressionLookup[actionGroup] = suppressedUsers;
            }

            if (!hasUsers)
            {
                suppressedUsers.Clear();
            }
            else
            {
                foreach (var user in userFilter)
                {
                    suppressedUsers.Add(user);
                }
            }
        }
    }

    public void Enable(int[]? actionsGroups, int[]? users)
    {
        var hasActionFilter = actionsGroups is not null && actionsGroups.Length > 0;
        var userFilter = users is null || users.Length is 0 
            ? [] 
            : _globalActionSuppression ? users : users.Where(user => _userGlobalSuppressionLookup.TryGetValue(user, out var isSuppressed) && isSuppressed);
        var hasUsers = userFilter.Any();

        // 1. Global unsuppression (Lift everything completely)
        if (!hasUsers && !hasActionFilter)
        {
            _globalActionSuppression = false;
            _userGlobalSuppressionLookup.Clear();
            _actionGroupSuppressionLookup.Clear();
            return;
        }

        // 2. Global user unsuppression (Remove specific users from global user block)
        if (hasUsers && !hasActionFilter)
        {
            foreach (var userId in userFilter)
            {
                _userGlobalSuppressionLookup[userId] = false;
                foreach (var suppressedUsers in _actionGroupSuppressionLookup.Values)
                {
                    suppressedUsers.Remove(userId);
                }
            }
            return;
        }

        // 3. Action Group specific unsuppression
        foreach (var actionGroup in actionsGroups!)
        {
            if (_actionGroupSuppressionLookup.TryGetValue(actionGroup, out var suppressedUsers))
            {
                if (!hasUsers)
                {
                    _actionGroupSuppressionLookup.Remove(actionGroup);
                }
                else
                {
                    foreach (var userId in userFilter)
                    {
                        suppressedUsers.Remove(userId);
                    }

                    if (suppressedUsers.Count == 0)
                    {
                        _actionGroupSuppressionLookup.Remove(actionGroup);
                    }
                }
            }
        }
    }

    public bool IsSuppressed(int userId, int actionGroup)
    {
        if (_globalActionSuppression)
        {
            return _userGlobalSuppressionLookup.TryGetValue(userId, out var suppressed)
                ? !suppressed
                : true;
        }
        if (_userGlobalSuppressionLookup.TryGetValue(userId, out var isSuppressed) && isSuppressed)
        {
            return true;
        }

        return _actionGroupSuppressionLookup.TryGetValue(actionGroup, out var userSuppressions)
            ? userSuppressions.Count is 0 || userSuppressions.Contains(userId)
            : false;
    }

    #endregion
}
