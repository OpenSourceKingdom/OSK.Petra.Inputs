using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Internal.Models;

internal class UserInputContext(int userId) : IUserInputContext
{
    #region Variables

    private readonly Dictionary<int, InputState> _inputStates = [];
    private readonly Dictionary<Type, CapabilityData> _features = [];

    #endregion

    #region IInputProcessingContext

    public int UserId => userId;

    public RuntimeDeviceIdentifier DeviceIdentifier { get; set; }

    public void SetFeature<TData>(TData data) 
        where TData : CapabilityData
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        _features[typeof(TData)] = data;
    }

    public TState GetOrCreateState<TState>(IInput input, Func<IInput, TState> factory)
        where TState : InputState
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        if (!_inputStates.TryGetValue(input.Id, out var state))
        {
            state = factory(input);
            _inputStates[input.Id] = state;
        }

        return (TState)state;
    }

    #endregion

    #region Helpers

    internal void Reset()
    {
        _inputStates.Clear();
        _features.Clear();
    }

    internal IEnumerable<CapabilityData> GetFeatures()
        => _features.Values;

    internal IEnumerable<InputState> GetStates()
        => _inputStates.Values;

    internal bool TryGetState(IInput input, out InputState state)
        => _inputStates.TryGetValue(input.Id,out state);

    #endregion
}
