using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Models;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// A feature that gives access to all pointer data for a given user
/// </summary>
public class PointerFeature: ICapabilityFeature
{
    #region Variables

    private readonly Dictionary<int, DevicePointer> _pointerLookup = [];

    #endregion

    #region Api

    /// <summary>
    /// The total number of pointers for the user
    /// </summary>
    public int TotalPointers => _pointerLookup.Count;

    /// <summary>
    /// Gets a <see cref="DevicePointer"/> from the user input feature by index
    /// </summary>
    /// <param name="index">The index to get the pointer</param>
    /// <returns>The device pointer</returns>
    public DevicePointer this[int index]
        => _pointerLookup[index];

    /// <summary>
    /// Gets the primary pointer (i.e. the pointer with the longest duration) across all devices
    /// </summary>
    /// <returns>The primary pointer, if it exists</returns>
    public DevicePointer? GetPrimaryPointer()
        => _pointerLookup.Values.OrderBy(d => d.Created).FirstOrDefault();

    /// <summary>
    /// Gets the primary pointer of a particular device
    /// </summary>
    /// <param name="deviceIdentifier">The device to get a primary pointer for</param>
    /// <returns>The primary pointer for the device, if it exists, otherwise null</returns>
    public DevicePointer? GetPrimaryPointer(RuntimeDeviceIdentifier deviceIdentifier)
        => _pointerLookup.Values.Where(d => d.DeviceIdentifier == deviceIdentifier).OrderBy(d => d.Created).FirstOrDefault();

    /// <summary>
    /// Gets all available pointers for the user
    /// </summary>
    /// <returns>The collection of pointers</returns>
    public IReadOnlyList<DevicePointer> GetPointers() 
        => [.. _pointerLookup.Values];

    /// <summary>
    /// Gets all available pointers associated with a specific device for the user
    /// </summary>
    /// <param name="deviceIdentifier">The device to get pointers for</param>
    /// <returns>The collection of pointers</returns>
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
