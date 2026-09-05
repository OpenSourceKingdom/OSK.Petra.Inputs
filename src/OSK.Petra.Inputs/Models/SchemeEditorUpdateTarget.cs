using OSK.Petra.Inputs.Ports;

namespace OSK.Petra.Inputs.Models;

/// <summary>
/// Specifies which part of the scheme editor has been updated and triggered an <see cref="ISchemeEditor.EditorUpdated"/> event.
/// </summary>
public enum SchemeEditorUpdateTarget
{
    /// <summary>
    /// The input configuration navigator changed, indicating a different
    /// input configuration is now selected.
    /// </summary>
    InputConfigurationNavigation,

    /// <summary>
    /// The action definition navigator changed, indicating a different action
    /// definition is now selected.
    /// </summary>
    DefinitionNavigation,

    /// <summary>
    /// The scheme navigator changed, indicating a different input scheme is
    /// now selected.
    /// </summary>
    SchemeNavigation,

    /// <summary>
    /// The device selection changed for a specific topology in the editor.
    /// </summary>
    DeviceSelection,

    /// <summary>
    /// The current scheme was successfully saved.
    /// </summary>
    SaveScheme,

    /// <summary>
    /// The current scheme was deleted.
    /// </summary>
    DeleteScheme,

    /// <summary>
    /// A new input scheme was created and is now selected in the editor.
    /// </summary>
    NewScheme
}
