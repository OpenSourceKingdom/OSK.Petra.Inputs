using Microsoft.Extensions.DependencyInjection;
using OSK.Expressions.Invoker;
using OSK.Extensions.Petra.Inputs.Configuration.Attributes;
using OSK.Extensions.Petra.Inputs.Configuration.Internal.Services;
using OSK.Extensions.Petra.Inputs.Configuration.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Linq;
using System.Reflection;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Configuration;

public static class InputSystemBuilderExtensions
{
    extension(IInputSystemConfigurationBuilder builder)
    {
        #region Definitions

        /// <inheritdoc cref="WithDefinition(IInputSystemConfigurationBuilder, string, Type, Action{IActionDefinitionBuilder})"/>
        public IInputSystemConfigurationBuilder WithDefinition<TDefinition>(string name)
            where TDefinition : class
            => builder.WithDefinition(name, typeof(TDefinition), _ => { });

        public IInputSystemConfigurationBuilder WithDefinition<TDefinition>(string name, Action<IActionDefinitionBuilder> definitionBuilderConfigurator)
            where TDefinition : class
            => builder.WithDefinition(name, typeof(TDefinition), definitionBuilderConfigurator);

        public IInputSystemConfigurationBuilder WithDefinition(string name, Type definitionType)
            => builder.WithDefinition(name, definitionType, _ => { });

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
        /// <param name="definitionBuilderConfigurator">Configure the definition builder</param>
        /// <returns>The builder for chaining</returns>
        public IInputSystemConfigurationBuilder WithDefinition(string name, Type definitionType, Action<IActionDefinitionBuilder> definitionBuilderConfigurator)
        {
            if (definitionType is null)
            {
                throw new ArgumentNullException(nameof(definitionType));
            }
            if (definitionBuilderConfigurator is null)
            {
                throw new ArgumentNullException(nameof(definitionBuilderConfigurator));
            }

            var definitionBuilder = new ActionDefinitionBuilder(name);

            foreach (var method in definitionType.GetActionMethods())
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

            definitionBuilderConfigurator(definitionBuilder);

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
