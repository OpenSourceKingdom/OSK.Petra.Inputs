using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Internal.Models;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Inputs.Internal.Services;

internal class DeviceInputContext(RuntimeDeviceIdentifier deviceIdentifier, IDeviceDescriptor deviceDescriptor)
{
    #region Variables

    private readonly Dictionary<long, InputState> _inputStates = [];

    #endregion

    #region Helpers

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    internal IDeviceDescriptor DeviceDescriptor => deviceDescriptor;

    internal bool TryGetInputState(long inputId, [NotNullWhen(true)] out InputState? state)
        => _inputStates.TryGetValue(inputId, out state);

    internal InputState GetOrCreateState(IDeviceInput input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (!_inputStates.TryGetValue(input.Id, out var state))
        {
            state = new DeviceInputState(this, input);
            _inputStates[input.Id] = state;
        }

        return state;
    }

    internal void RemoveState(DeviceInputState state)
    {
        _inputStates.Remove(state.DeviceInput.Id);
    }

    internal IEnumerable<InputState> GetInputStateSnapshot()
        => [.. _inputStates.Values];

    #endregion
}
