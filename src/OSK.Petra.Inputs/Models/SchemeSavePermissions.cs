namespace OSK.Petra.Inputs.Models;

public enum SchemeSavePermissions
{
    /// <summary>
    /// No special save behavior. If a file already exists, the save will fail.
    /// </summary>
    None = 0,

    /// <summary>
    /// The saving operation should overwrite any existing scheme with the same name
    /// </summary>
    Overwrite = 1
}
