using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Internal.Models;

internal class UserInputContext(int userId)
{
    #region Variables

    private bool _globalActionSuppression;
    private InputScheme? _scheme;
    private readonly Dictionary<int, bool> _suppressedActions = [];
    private readonly Dictionary<RuntimeDeviceIdentifier, DeviceInputContext> _deviceContexts = [];

    #endregion

    #region Api

    internal bool IsGloballySuppressed => _globalActionSuppression;

    public int UserId => userId;

    public IEnumerable<DeviceInputContext> DeviceInputContexts => _deviceContexts.Values;

    public InputScheme? Scheme 
    { 
        get => _scheme;
        set
        {
            _scheme = value;
            _deviceContexts.Clear();
        }
    }

    public SchemeEditorDelay? EditorInputCaptureTimeout { get; set; }

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

    public bool IsSuppressed(int? actionGroup)
        => _globalActionSuppression || (actionGroup.HasValue && _suppressedActions.TryGetValue(actionGroup.Value, out var isSuppressed) && isSuppressed);

    public DeviceInputContext GetOrAddDevice(RuntimeDeviceIdentifier deviceIdentifier, Func<RuntimeDeviceIdentifier, IDeviceDescriptor> deviceFactory)
    {
        if (deviceFactory is null)
        {
            throw new ArgumentNullException(nameof(deviceFactory));
        }
        if (_deviceContexts.TryGetValue(deviceIdentifier, out var deviceInputContext))
        {
            return deviceInputContext;
        }

        deviceInputContext = new(userId, deviceIdentifier, deviceFactory(deviceIdentifier));
        _deviceContexts[deviceIdentifier] = deviceInputContext;

        return deviceInputContext;
    }

    #endregion
}
