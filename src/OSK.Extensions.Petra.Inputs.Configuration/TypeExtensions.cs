using OSK.Extensions.Petra.Inputs.Configuration.Attributes;
using OSK.Petra.Inputs.Abstractions.Configuration;
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
        /// <summary>
        /// Gets a collection of executable methods from the type that satisfies the expectations of an <see cref="ActionDefinition"/> to create a <see cref="InputAction"/>
        /// </summary>
        /// <remarks>
        /// 💡Notes:
        /// <list type="bullet">
        /// <item>Input action methods are expected to follow these guidelines: 1) they must be a void return 2) They must only contain a single parameter for the <see cref="IInputEventContext"/>, and 3) the method names should be unique (overloads should be avoided)</item>
        /// <item>For customizing various input action configuration, please utilize the <see cref="InputActionAttribute"/></item>
        /// </list>
        /// </remarks>
        /// <returns>The collection of methods that satisfies the expectations of an input action</returns>
        public IEnumerable<MethodInfo> GetActionMethods()
            => type.GetMethods().Where(method =>
            {
                var methodParameters = method.GetParameters();
                return methodParameters.Length is 1 && methodParameters[0].ParameterType == typeof(IInputEventContext)
                 && method.ReturnParameter.ParameterType == typeof(void);
            });
    }
}
