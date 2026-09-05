namespace OSK.Petra.Inputs.Models;

/// <summary>
/// Specifies permission flags that control how input schemes can be saved to storage.
/// </summary>
public enum SchemeSavePermissions
{
    /// <summary>
    /// No special save behavior. If a file already exists with the same name, it will not overwritten.
    /// </summary>
    None = 0,

    /// <summary>
    /// Allow the save operation to overwrite any existing scheme with the same name.
    /// </summary>
    Overwrite = 1
}
