using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Linq;
using OSK.Petra.Inputs.Internal.Models;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Internal.Services;

internal class UserInputContext(int userId): IUserInputContext
{
    #region Variables

    private bool _globalActionSuppression;
    private InputScheme? _scheme;
    private readonly Dictionary<int, bool> _suppressedActions = [];
    private readonly Dictionary<DeviceIdentity, DeviceInputContext> _deviceContexts = [];
    private readonly Dictionary<Type, ICapabilityFeature> _features = [];

    private readonly VirtualInputContext _virtualInputContext = new();

    #endregion

    #region IUserInputContext

    public int UserId => userId;

    public IVirtualInputContext VirtualInputContext => _virtualInputContext;

    public bool TryGetInputState(DeviceIdentity identity, long inputId, [NotNullWhen(true)] out IInputState? state)
    {
        state = null;
        if (!_deviceContexts.TryGetValue(identity, out var context))
        {
            return false;
        }

        if (context.TryGetInputState(inputId, out var inputState))
        {
            state = inputState;
            return true;
        }

        return false;
    }

    public void SetFeature<TData>(TData data)
        where TData : ICapabilityFeature
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        _features[typeof(TData)] = data;
    }

    public TFeature? GetFeature<TFeature>()
        where TFeature : ICapabilityFeature
        => _features.TryGetValue(typeof(TFeature), out var feature) && feature is TFeature typedFeature
            ? typedFeature
            : default;

    #endregion

    #region Api

    internal bool IsGloballySuppressed => _globalActionSuppression;

    public IEnumerable<InputState> GetInputStateSnapshot() 
        => _deviceContexts.Values.SelectMany(context => context.GetInputStateSnapshot())
            .Concat(_virtualInputContext.GetInputStateSnapshot());

    public InputScheme? Scheme 
    { 
        get => _scheme;
        set
        {
            _scheme = value;
            _virtualInputContext.Initialize(_scheme);
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
        if (_deviceContexts.TryGetValue(deviceIdentifier.DeviceIdentity, out var deviceInputContext))
        {
            return deviceInputContext;
        }

        deviceInputContext = new(deviceIdentifier, deviceFactory(deviceIdentifier));
        _deviceContexts[deviceIdentifier.DeviceIdentity] = deviceInputContext;

        return deviceInputContext;
    }

    #endregion
}
