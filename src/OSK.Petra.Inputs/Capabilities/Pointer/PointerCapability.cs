using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Internal.Models;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// A capability that is able to process <see cref="PointerEvent"/>
/// </summary>
/// <param name="options"></param>
public class PointerCapability(ICapabilityOptions<PointerCapabilityOptions> options) : InputCapability<PointerEvent, PointerSettings>
{
    #region InputCapability Overrides

    protected override void Process(IUserInputContext context, IInputState state, PointerEvent pointerEvent, PointerSettings settings, TimeSpan deltaTime)
    {
        switch (state)
        {
            case DeviceInputState deviceInputState:
                var details = deviceInputState.GetOrCreateDetails(() => new PointerDetails(pointerEvent.Position, options.Value.MaxPositionEntries, settings.DistanceThreshold));
                if (deviceInputState.IsNewActivation)
                {
                    var feature = context.GetOrCreateFeature<PointerFeature>();

                    feature.AddDetails(deviceInputState, details);

                    // Pointers are always active, if they exist. 
                    deviceInputState.CombinePhase(InputPhase.Active);
                    return;
                }

                details.UpdatePosition(pointerEvent.Position, deviceInputState.Duration);
                break;
        }
    }

    #endregion
}
