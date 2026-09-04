using System;

namespace OSK.Petra.Inputs.Abstractions.Devices;

public readonly struct DeviceInputIdentifier(DeviceIdentity deviceIdentity, long inputId)
{
    #region Variables

    public DeviceIdentity DeviceIdentity => deviceIdentity;

    public long InputId => inputId;

    #endregion

    #region Helpers

    public bool Matches(DeviceInputIdentifier identifier, bool allowGenericMatch = true)
        => Matches(identifier.DeviceIdentity, identifier.InputId, allowGenericMatch);

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
