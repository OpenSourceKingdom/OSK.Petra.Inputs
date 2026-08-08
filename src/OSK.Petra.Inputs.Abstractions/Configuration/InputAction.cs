using System;
using System.Collections.Generic;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions.Configuration;

/// <summary>
/// Represents an action that can be executed in the input system. This is configured and then retrieved to execute functions
/// in an application that integrates with the input system.
/// </summary>
/// <param name="actionName">The unique name for the action</param>
/// <param name="triggerPhases">The phases considered valid to trigger the action</param>
/// <param name="actionExecutor">The specific action to execute</param>
/// <param name="description">A readable description for the action that can be displayed for users</param>
/// <param name="actionGroup">An option group number that specifies the action group this action belongs to</param>
public class InputAction(string actionName, ISet<InputPhase> triggerPhases, Action<IInputEventContext> actionExecutor, string? description = null, int? actionGroup = null)
{
    #region Api

    /// <summary>
    /// A unique action name
    /// </summary>
    public string Name => actionName;

    /// <summary>
    /// Helper text to be displayed with the action on a device scheme settings screen or similar
    /// </summary>
    public string? Description => description;

    /// <summary>
    /// The specific input phases that will trigger this action
    /// </summary>
    public ISet<InputPhase> TriggerPhases => triggerPhases;

    /// <summary>
    /// Specifies an action group for the input action. This can be used in conjunction with notifications to ignore
    /// actions of a given type during input processing
    /// </summary>
    public int? ActionGroup => actionGroup;

    /// <summary>
    /// The configured action to execute when the related input is activated
    /// </summary>
    public Action<IInputEventContext> ActionExecutor => actionExecutor;

    #endregion
}
