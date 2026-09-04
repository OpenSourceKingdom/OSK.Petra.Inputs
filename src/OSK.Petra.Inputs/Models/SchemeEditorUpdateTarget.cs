namespace OSK.Petra.Inputs.Models;

/// <summary>
/// Specifies which part of the scheme editor has been updated.
/// </summary>
public enum SchemeEditorUpdateTarget
{
    /// <summary>
    /// The input configuration navigator changed.
    /// </summary>
    InputConfigurationNavigation,

    /// <summary>
    /// The definition navigator changed.
    /// </summary>
    DefinitionNavigation,

    /// <summary>
    /// The scheme navigator changed.
    /// </summary>
    SchemeNavigation,

    /// <summary>
    /// The device selection changed.
    /// </summary>
    DeviceSelection,

    /// <summary>
    /// The current scheme was saved.
    /// </summary>
    SaveScheme,

    /// <summary>
    /// The current scheme was deleted.
    /// </summary>
    DeleteScheme,

    /// <summary>
    /// A new scheme was created.
    /// </summary>
    NewScheme
}
