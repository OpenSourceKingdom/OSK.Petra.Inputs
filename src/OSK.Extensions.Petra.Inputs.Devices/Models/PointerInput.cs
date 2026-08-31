using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class PointerInput: Input, IInput<PointerSettings>
{
    #region Constructors

    protected PointerInput(long id)
        : this(id, .1f)
    {
    }

    protected PointerInput(long id, float distanceThreshold)
    : base(id)
    {
        Settings = new PointerSettings()
        {
            DistanceThreshold = distanceThreshold
        };
    }

    #endregion

    #region IInput

    public PointerSettings Settings { get; private set; }

    #endregion

    #region Api

    public void SetDistanceThreshold(float threshold)
        => Settings = new()
        {
            DistanceThreshold = threshold < 0 ? 0 : threshold
        };

    #endregion
}
