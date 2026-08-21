using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Internal.Models;

internal class DeviceInputContext(int userId, RuntimeDeviceIdentifier deviceIdentifier, IDeviceDescriptor deviceDescriptor) : IDeviceInputContext
{
    #region Variables

    private readonly Dictionary<int, InputState> _inputStates = [];
    private readonly Dictionary<Type, ICapabilityFeature> _features = [];

    #endregion

    #region IInputProcessingContext

    public int UserId => userId;

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

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

    #region Helpers

    internal IDeviceDescriptor DeviceDescriptor => deviceDescriptor;

    internal InputState GetOrCreateState(IInput input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (!_inputStates.TryGetValue(input.Id, out var state))
        {
            state = new(input, this);
            _inputStates[input.Id] = state;
        }

        return state;
    }

    internal void RemoveState(InputState state)
    {
        _inputStates.Remove(state.Input.Id);
    }

    internal IEnumerable<InputState> GetInputStateSnapshot()
        => [.. _inputStates.Values];

    internal void Reset()
    {
        _inputStates.Clear();
        _features.Clear();
    }

    #endregion
}
