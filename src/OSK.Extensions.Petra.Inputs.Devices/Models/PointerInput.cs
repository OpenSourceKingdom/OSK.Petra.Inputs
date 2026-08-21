using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Capabilities.Pointer;

namespace OSK.Extensions.Petra.Inputs.Devices.Models;

public abstract class PointerInput: Input, IInput<PointerSettings>
{
    #region Constructors

    protected PointerInput(int id)
    : this(id, false)
    {
    }

    protected PointerInput(int id, bool allowReactivation)
        : base(id)
    {
        Settings = new PointerSettings()
        {
            DistanceThreshold = .1f
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
