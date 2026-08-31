using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

public class InputCapabilityOptionConfiguration(IEnumerable<CapabilityOptions> capabilityOptions)
{
    #region Variables

    private readonly Dictionary<Type, CapabilityOptions> _options = capabilityOptions.ToDictionary(o => o.GetType());

    #endregion

    #region Api

    public bool TryGetOptions<TOptions>([NotNullWhen(true)] out TOptions? options)
        where TOptions : CapabilityOptions
    {
        if (!_options.TryGetValue(typeof(TOptions), out var setOptions) || setOptions is not TOptions typedOptions)
        {
            options = default;
            return false;
        }

        options = typedOptions;
        return true;
    }

    #endregion
}
