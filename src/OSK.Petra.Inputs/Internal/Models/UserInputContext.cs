using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Internal.Models;

internal class UserInputContext(int userId, InputScheme scheme)
{
    #region Variables

    private bool _globalActionSuppression;
    private readonly Dictionary<int, bool> _suppressedActions = [];
    private readonly Dictionary<RuntimeDeviceIdentifier, DeviceInputContext> _deviceContexts = [];

    #endregion

    #region Api

    public int UserId => userId;

    public IEnumerable<DeviceInputContext> DeviceInputContexts => _deviceContexts.Values;

    public InputScheme Scheme 
    { 
        get => scheme;
        set
        {
            scheme = value;
            _deviceContexts.Clear();
        }
    }

    public SchemeEditorDelay? EditorDelay { get; set; }

    public void Suppress(int[]? actionGroups, bool isSuppressed)
    {
        if (actionGroups is null || actionGroups.Length is 0)
        {
            _globalActionSuppression = isSuppressed;
            _suppressedActions.Clear();

            return;
        }

        foreach (var actionGroup in actionGroups)
        {
            _suppressedActions[actionGroup] = isSuppressed;
        }
    }

    public bool IsSuppressed(int actionGroup)
        => (_suppressedActions.TryGetValue(actionGroup, out var isSuppressed) && isSuppressed) || _globalActionSuppression;

    public DeviceInputContext GetOrAddDevice(RuntimeDeviceIdentifier deviceIdentifier)
    {
        if (_deviceContexts.TryGetValue(deviceIdentifier, out var deviceInputContext))
        {
            return deviceInputContext;
        }

        deviceInputContext = new(userId, deviceIdentifier);
        _deviceContexts[deviceIdentifier] = deviceInputContext;

        return deviceInputContext;
    }

    #endregion
}
