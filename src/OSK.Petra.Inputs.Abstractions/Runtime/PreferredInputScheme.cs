namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// Represents a user's preferred input scheme, and is used with a <see cref="ISchemeRepository"/> for data persistence.
/// </summary>
public readonly struct PreferredInputScheme
{
    public required int UserId { get; init; }

    public required string DefinitionName { get; init; }

    public required string ConfigurationId { get; init; }

    public required string SchemeName { get; init; }
}
