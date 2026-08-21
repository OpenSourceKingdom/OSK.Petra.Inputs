using OSK.Petra.Inputs.Abstractions.Inputs;
using System;
using System.Linq.Expressions;

namespace OSK.Extensions.Petra.Inputs.Configuration.Ports;

internal interface IInputSchemeBuilder<TDefinition>
{
    string Name { get; }

    IInputSchemeBuilder<TDefinition> MakeDefault();

    IInputSchemeBuilder<TDefinition> WithMap(DeviceIdentity deviceIdentity, IInput input, Expression<Action<TDefinition>> methodExpression);
}
