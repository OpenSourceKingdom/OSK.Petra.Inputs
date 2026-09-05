namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// Represents a user's preferred input scheme, and is used with a <see cref="ISchemeRepository"/> for data persistence.
/// </summary>
public readonly struct PreferredInputScheme
{
    /// <summary>
    /// The user this preference is set for
    /// </summary>
    public required int UserId { get; init; }

    /// <summary>
    /// The specific input configuration this preference refers to
    /// </summary>
    public required string ConfigurationId { get; init; }

    /// <summary>
    /// The definition this preference refers to
    /// </summary>
    public required string DefinitionName { get; init; }

    /// <summary>
    /// The specific scheme this preference refers to
    /// </summary>
    public required string SchemeName { get; init; }
}
