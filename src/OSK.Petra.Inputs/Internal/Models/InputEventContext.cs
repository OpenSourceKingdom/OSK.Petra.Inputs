using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Internal.Models;

internal class InputEventContext(int userId, RuntimeDeviceIdentifier deviceIdentifier, DeviceInputContext deviceContext, InputState state, TimeSpan deltaTime, IServiceProvider services) : IInputEventContext
{
    #region IInputEventContext

    public int UserId => userId;

    public TimeSpan DeltaTime => deltaTime;

    public RuntimeDeviceIdentifier DeviceIdentifier => deviceIdentifier;

    public IInput Input => state.Input;

    public IServiceProvider Services => services;

    public TFeature? GetDeviceFeature<TFeature>()
        where TFeature : ICapabilityFeature
        => deviceContext.GetFeature<TFeature>();

    public TDetail? GetInputDetail<TDetail>() 
        where TDetail : ICapabilityDetail
        => state.GetDetail<TDetail>();

    #endregion
}
