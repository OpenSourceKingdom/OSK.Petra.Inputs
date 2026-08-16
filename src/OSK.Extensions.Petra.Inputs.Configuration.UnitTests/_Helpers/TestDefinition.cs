using OSK.Extensions.Petra.Inputs.Configuration.Attributes;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Extensions.Petra.Inputs.Configuration.UnitTests._Helpers;

internal class TestDefinition
{
    [InputAction(ActionName = "CustomName", TriggerPhases = new[] { InputPhase.Start, InputPhase.End })]
    public void MarkedMethod(IInputEventContext context) { }

    public void UnmarkedMethod(IInputEventContext context) { }

    public string MethodWithReturnValue(IInputEventContext context) => "";

    public void MethodWithTwoParams(IInputEventContext context, int extra) { }

    public void MethodWithWrongParamType(string param) { }
}
