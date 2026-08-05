using Microsoft.Extensions.Options;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class PointerCapability(IOptions<PointerCapabilityOptions> options) : InputCapability<IPointer>
{
    #region InputCapability Overrides

    protected override void Process(IDeviceInputContext context, IInputState state, IPointer input, TimeSpan deltaTime)
    {
        var details = state.GetOrCreateDetails(() => new PointerDetails(input.Position, options.Value.MaxPositionEntries, input.Settings.DistanceThresholdd));
        if (state.IsNewActivation)
        {
            var feature = context.GetOrCreateFeature<PointerFeature>();

            feature.AddDetails(state, details);

            // Pointers are always active, if they exist. 
            state.CombinePhase(InputPhase.Active);
            return;
        }

         details.UpdatePosition(input.Position, state.Duration);
    }

    #endregion
}
