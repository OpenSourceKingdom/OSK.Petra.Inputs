using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Inputs.Notifications;

/// <summary>
/// Notifies the system to modify the suppression of action groups for users.
/// </summary>
public class ModifyActionGroupSuppressionNotification: SystemNotification
{
    /// <summary>
    /// Sets whether the specified action groups should be suppressed for the specified users.
    /// When suppressed, input actions assigned to those action groups will not be triggered for the affected users.
    /// See <see cref="InputAction.ActionGroup"/> for assigning an action to an action group.
    /// </summary>
    public bool Suppress { get; init; }

    /// <summary>
    /// The users whose action group suppression state should be modified.
    /// If <see langword="null"/> or empty, the suppression state will be modified for all users.
    /// </summary>
    public int[]? UserIds { get; init; }

    /// <summary>
    /// The action groups whose suppression state should be modified.
    /// If <see langword="null"/> or empty, the suppression state will be modified for all action groups.
    /// </summary>
    public int[]? ActionGroups { get; init; }
}
