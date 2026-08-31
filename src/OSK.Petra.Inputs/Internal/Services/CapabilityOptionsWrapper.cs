using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Ports;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Internal.Services;

internal class CapabilityOptionsWrapper<TCapabilityOptions>(ICapabilityOptionsProvider provider) : ICapabilityOptions<TCapabilityOptions>
    where TCapabilityOptions : CapabilityOptions, new()
{
    #region ICapabilityOptions

    public TCapabilityOptions Value => provider.Get<TCapabilityOptions>();

    #endregion
}
