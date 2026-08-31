using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class TypeExtensions
{
    extension(Type type)
    {
        public IEnumerable<MethodInfo> GetActionMethods()
            => type.GetMethods().Where(method =>
            {
                var methodParameters = method.GetParameters();
                return methodParameters.Length is 1 && methodParameters[0].ParameterType == typeof(IInputEventContext)
                 && method.ReturnParameter.ParameterType == typeof(void);
            });
    }
}
