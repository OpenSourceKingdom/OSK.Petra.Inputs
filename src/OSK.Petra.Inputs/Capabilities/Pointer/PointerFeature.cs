using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class PointerFeature: ICapabilityFeature
{
    #region Variables

    private readonly Dictionary<int, DevicePointer> _pointerLookup = [];

    #endregion

    #region Api

    public int TotalPointers => _pointerLookup.Count;

    public DevicePointer this[int index]
        => _pointerLookup[index];

    public DevicePointer? GetPrimaryPointer()
        => _pointerLookup.Values.OrderBy(d => d.Created).FirstOrDefault();

    public DevicePointer? GetPrimaryPointer(RuntimeDeviceIdentifier deviceIdentifier)
        => _pointerLookup.Values.Where(d => d.DeviceIdentifier == deviceIdentifier).OrderBy(d => d.Created).FirstOrDefault();

    public IReadOnlyList<DevicePointer> GetPointers() 
        => [.. _pointerLookup.Values];

    public IReadOnlyList<DevicePointer> GetPointers(RuntimeDeviceIdentifier deviceIdentifier)
        => [.. _pointerLookup.Values.Where(d => d.DeviceIdentifier == deviceIdentifier)];

    #endregion

    #region Helpers

    internal void AddDetails(DeviceInputState state, PointerDetails pointerDetails)
    {
        state.Disposed += RemoveDetails;

        var nextId = 0;
        while (_pointerLookup.ContainsKey(nextId))
        {
            nextId++;
        }

        _pointerLookup[nextId] = new DevicePointer(nextId, state.DeviceIdentifier, state.DeviceInput.Id, pointerDetails);
    }

    private void RemoveDetails(IInputState state)
    {
        if (state is DeviceInputState deviceInputState)
        {
            var kvpQuery = _pointerLookup.Where(pointerKvp 
                => pointerKvp.Value.DeviceIdentifier == deviceInputState.DeviceIdentifier && pointerKvp.Value.DevicePointerId == deviceInputState.DeviceInput.Id);
            if (kvpQuery.Any())
            {
                _pointerLookup.Remove(kvpQuery.First().Key);
            }
        }
    }

    #endregion
}
