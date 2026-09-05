using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents a collection of option configurations used with an input system's <see cref="IInputCapability"/> collection.
/// </summary>
/// <param name="capabilityOptions"></param>
public class InputCapabilityOptionConfiguration(IEnumerable<CapabilityOptions> capabilityOptions)
{
    #region Variables

    private readonly Dictionary<Type, CapabilityOptions> _options = capabilityOptions.ToDictionary(o => o.GetType());

    #endregion

    #region Api

    /// <summary>
    /// Attempts to get a specific <see cref="CapabilityOptions"/>, if it was configured during initialization of the input system
    /// </summary>
    /// <typeparam name="TOptions">The capability options type</typeparam>
    /// <param name="options">The options, if configured</param>
    /// <returns>Whether the custom options exist in the configuration</returns>
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
