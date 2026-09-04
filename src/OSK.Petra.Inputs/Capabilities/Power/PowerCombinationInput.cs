using OSK.Petra.Inputs.Abstractions.Devices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Capabilities.Power;

public class PowerCombinationInput(IEnumerable<DeviceInputIdentifier> inputIdentifiers) : VirtualInput<PowerCombinationInput>, IPowerCombinationInput
{
    #region VirtualInput Overrides

    protected override bool Equals(PowerCombinationInput other)
        => other is not null && InputIdentifiers.SequenceEqual(other.InputIdentifiers);

    #endregion

    #region IPowerCombinationInput

    public IReadOnlyCollection<DeviceInputIdentifier> InputIdentifiers { get; } = inputIdentifiers is null 
        ? [] 
        : [.. inputIdentifiers.Distinct()
                              .OrderBy(id => id.DeviceIdentity.TopologyName.Name)
                              .ThenBy(id => id.DeviceIdentity.DeviceFamily.Name)
                              .ThenBy(id => id.DeviceIdentity.Name)
                              .ThenBy(id => id.InputId)];

    public override async Task<IEnumerable<InputGlyph>> GetGlyphsAsync(DeviceCatalog deviceCatalog, CancellationToken cancellationToken = default)
    {
        var glyphs = new List<InputGlyph>();
        foreach (var inputIdentifier in InputIdentifiers)
        {
            var device = deviceCatalog.GetDevice(inputIdentifier.DeviceIdentity);
            if (device is null) 
            {
                return [];
            }

            var input = device.GetInput(inputIdentifier.InputId);
            if (input is null)
            {
                return [];
            }

            var glyph = await input.GetGlyphAsync(cancellationToken);
            glyphs.Add(glyph);
        }

        return glyphs;
    }

    #endregion
}
