using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Internal.Models;

internal class UserInputContext(int userId) : IUserInputContext
{
    #region Variables

    internal Dictionary<Type, CapabilityData> Features { get; } = [];

    #endregion

    #region IInputProcessingContext

    public int UserId => userId;

    public required RuntimeDeviceIdentifier DeviceIdentifier { get; set; }

    public required IInput Input { get; set; }

    public void SetFeature<TData>(TData data) 
        where TData : CapabilityData
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        Features[typeof(TData)] = data;
    }

    #endregion
}
