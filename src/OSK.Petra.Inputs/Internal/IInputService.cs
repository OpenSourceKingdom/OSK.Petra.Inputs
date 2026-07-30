using System;

namespace OSK.Petra.Inputs.Internal;

internal interface IInputService
{
    bool PauseInput { get; set; }

    void Update(TimeSpan delta);
}
