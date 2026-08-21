using Microsoft.Extensions.Options;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class PointerCapability(IOptions<PointerCapabilityOptions> options) : InputCapability<PointerEvent, PointerSettings>
{
    #region InputCapability Overrides

    protected override void Process(IDeviceInputContext context, IInputState state, PointerEvent pointerEvent, PointerSettings settings, TimeSpan deltaTime)
    {
        var details = state.GetOrCreateDetails(() => new PointerDetails(pointerEvent.Position, options.Value.MaxPositionEntries, settings.DistanceThreshold));
        if (state.IsNewActivation)
        {
            var feature = context.GetOrCreateFeature<PointerFeature>();

            feature.AddDetails(state, details);

            // Pointers are always active, if they exist. 
            state.CombinePhase(InputPhase.Active);
            return;
        }

         details.UpdatePosition(pointerEvent.Position, state.Duration);
    }

    #endregion
}
