using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Internal.Models;

internal class InputEventContext(int userId, IUserInputContext userInputContext, InputOriginationSource originationSource, InputState state, TimeSpan deltaTime,
    IServiceProvider services) : IInputEventContext
{
    #region IInputEventContext

    public int UserId => userId;

    public TimeSpan DeltaTime => deltaTime;

    public InputOriginationSource OriginationSource => originationSource;

    public IServiceProvider Services => services;

    public TFeature? GetInputFeature<TFeature>()
        where TFeature : ICapabilityFeature
        => userInputContext.GetFeature<TFeature>();

    public TDetails? GetInputDetails<TDetails>() 
        where TDetails : ICapabilityDetails
        => state.GetDetails<TDetails>();

    #endregion
}
