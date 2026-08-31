namespace OSK.Petra.Inputs.Abstractions.Devices;

public static class InputPhaseExtensions
{
    extension(InputPhase phase)
    {
        public InputPhase Combine(InputPhase other)
        {
            if (phase is InputPhase.End || other is InputPhase.End)
            {
                return InputPhase.End;
            }
            if (phase is InputPhase.Start || other is InputPhase.Start)
            {
                return InputPhase.Start;
            }

            return InputPhase.Active;
        }
    }
}
