using Microsoft.Extensions.DependencyInjection;
using OSK.Expressions.Invoker;
using OSK.Extensions.Petra.Inputs.Configuration.Attributes;
using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Linq;
using System.Reflection;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class InputSystemBuilderExtensions
{
    extension(IInputSystemBuilder builder)
    {
        #region Definitions

        /// <inheritdoc cref="WithDefinition(IInputSystemBuilder, string, Type)"/>
        public IInputSystemBuilder WithDefinition<TDefinition>(string name)
            where TDefinition : class
            => builder.WithDefinition(name, typeof(TDefinition));

        /// <summary>
        /// The extension searches for methods based on their return type being void and taking
        /// a single parameter for <see cref="IInputEventContext"/>. Names must be unique, so
        /// overloads are not guaranteed to work. If changing the name of the method or enabling extra
        /// features, like pointer details, is needed, you will need to use the <see cref="InputActionAttribute"/>
        /// to define these options.
        /// <param name="name">The name of the definition to add</param>
        /// </summary>
        /// <param name="definitionType">
        /// The definition type that will be used to get the methods for. This service needs to be registered on the DI chain that the input system uses.
        /// </param>
        /// <returns>The builder for chaining</returns>
        public IInputSystemBuilder WithDefinition(string name, Type definitionType)
        {
            if (definitionType is null)
            {
                throw new ArgumentNullException(nameof(definitionType));
            }

            var methodsToRegister = definitionType.GetMethods().Where(method =>
            {
                var methodParameters = method.GetParameters();
                return methodParameters.Length is 1 && methodParameters[0].ParameterType == typeof(IInputEventContext)
                 && method.ReturnParameter.ParameterType == typeof(void);
            });

            var definitionBuilder = new ActionDefinitionBuilder(name);

            foreach (var method in methodsToRegister)
            {
                var inputActionAttribute = method.GetCustomAttribute<InputActionAttribute>();
                var inputActionName = string.IsNullOrWhiteSpace(inputActionAttribute?.ActionName)
                    ? method.Name
                    : inputActionAttribute.ActionName;
                var invoker = InvokerFactory.CreateInvoker(definitionType, method);

                definitionBuilder.WithAction(new InputAction(inputActionName,
                    inputActionAttribute?.TriggerPhases.ToHashSet() ?? [InputPhase.Start],
                    inputEventContext => invoker.FastInvoke(inputEventContext.Services.GetRequiredService(definitionType), [inputEventContext]),
                    inputActionAttribute?.Description, inputActionAttribute?.InternalActionGroup));
            }

            var output = definitionBuilder.Build();
            builder.WithActionDefinition(output.Definition);

            foreach (var scheme in output.Schemes)
            {
                builder.WithInputScheme(scheme);
            }

            return builder;
        }

        #endregion
    }
}
