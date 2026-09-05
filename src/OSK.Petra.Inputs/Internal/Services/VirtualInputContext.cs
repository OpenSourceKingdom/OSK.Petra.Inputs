using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OSK.Petra.Inputs.Internal.Services;

internal class VirtualInputContext : IVirtualInputContext
{
    #region Variables

    private IVirtualInput[] _inputs = [];
    private Dictionary<IVirtualInput, VirtualInputState> _states = [];

    #endregion

    #region IVirtualInputContext

    public IEnumerable<TInput> GetInputs<TInput>()
        where TInput : IVirtualInput
        => _inputs.OfType<TInput>();

    public IInputState GetOrCreateState(IVirtualInput virtualInput, Func<IInputEvent[]> inputEventsFactory)
    {
        if (virtualInput is null)
        {
            throw new ArgumentNullException(nameof(virtualInput));
        }
        if (inputEventsFactory is null)
        {
            throw new ArgumentNullException(nameof(inputEventsFactory));
        }

        if (!_states.TryGetValue(virtualInput, out var state))
        {
            state = new VirtualInputState(this, virtualInput)
            {
                LastReceivedEvents = inputEventsFactory()
            };

            _states[virtualInput] = state;
        }

        return state;
    }

    public bool TryGetState(IVirtualInput virtualInput, [NotNullWhen(true)] out IInputState? state)
    {
        if (_states.TryGetValue(virtualInput, out var virtualState))
        {
            state = virtualState;
            return true;
        }

        state = null;
        return false;
    }

    #endregion

    #region Api

    public IEnumerable<InputState> GetInputStateSnapshot()
        => _states.Values;

    public void Initialize(InputScheme? scheme)
    {
        _inputs = scheme?.VirtualMaps.Select(map => map.VirtualInput).ToArray() ?? [];
        _states.Clear();
    }

    public void RemoveState(VirtualInputState state)
    {
        _states.Remove(state.VirtualInput);
    }

    #endregion
}
