namespace OSK.Extensions.Petra.Inputs.Configuration.Options;

/// <summary>
/// A set of options specific to input actions
/// </summary>
public class InputActionOptions
{
    /// <summary>
    /// An optional grouping that can be used to group specific input actions together. i.e. group pointer actions or similar so that they can be manipulated as group. This is best used when wanting to suppress
    /// groups of input, as an example.
    /// </summary>
    public int? ActionGroup { get; set; }

    /// <summary>
    /// Describes the input action, best utilized with UI for input configuration
    /// </summary>
    public string? Description { get; set; }
}
