using System;

namespace OSK.Petra.Inputs.Abstractions.Devices;

/// <summary>
/// A identifier for an input on a device
/// </summary>
/// <param name="deviceIdentity">The device the input belongs to</param>
/// <param name="inputId">The id for the input</param>
public readonly struct DeviceInputIdentifier(DeviceIdentity deviceIdentity, long inputId)
{
    #region Variables

    /// <summary>
    /// The device the input belongs to
    /// </summary>
    public DeviceIdentity DeviceIdentity => deviceIdentity;

    /// <summary>
    /// The unique id for the input
    /// </summary>
    public long InputId => inputId;

    #endregion

    #region Helpers

    /// <summary>
    /// Validates whether the provided identifier matches this identifier
    /// </summary>
    /// <param name="identifier">The identifier to validate against</param>
    /// <param name="allowGenericMatch">Whether generic families and devices should be considered matches</param>
    /// <returns>Whether the identifiers matched</returns>
    public bool Matches(DeviceInputIdentifier identifier, bool allowGenericMatch = true)
        => Matches(identifier.DeviceIdentity, identifier.InputId, allowGenericMatch);

    /// <summary>
    /// Validates whether the provided de mavice identity and input id matches this identifier
    /// </summary>
    /// <param name="identity">The device to validate against</param>
    /// <param name="inputId">The input id to validate against</param>
    /// <param name="allowGenericMatch">Whether generic families and devices should be considered matches</param>
    /// <returns>Whether the identifier matched</returns>
    public bool Matches(DeviceIdentity identity, long inputId, bool allowGenericMatch = true)
    {
        if (InputId != inputId)
        {
            return false;
        }
        if (DeviceIdentity.TopologyName != identity.TopologyName)
        {
            return false;
        }

        if (DeviceIdentity.DeviceFamily == DeviceFamily.Generic || identity.DeviceFamily == DeviceFamily.Generic)
        {
            return allowGenericMatch;
        }
        if (DeviceIdentity.DeviceFamily != identity.DeviceFamily)
        {
            return false;
        }

        if (DeviceIdentity.Name.Equals(DeviceIdentities.GenericDeviceName, StringComparison.OrdinalIgnoreCase) 
                || identity.Name.Equals(DeviceIdentities.GenericDeviceName, StringComparison.OrdinalIgnoreCase))
        {
            return allowGenericMatch;
        }

        return DeviceIdentity.Name.Equals(identity.Name, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
