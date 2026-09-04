using OSK.Petra.Inputs.Abstractions.Runtime;
using System;

namespace OSK.Petra.Inputs.Abstractions;

public abstract class InputCapability<TInputEvent> : IInputCapability
    where TInputEvent : IInputEvent
{
    #region IInputCapability

    public bool CanProcess(IInputEvent inputEvent)
        => inputEvent is TInputEvent;

    public void Process(IUserInputContext context, IInputState state, IInputEvent inputEvent, TimeSpan deltaTime)
    {
        if (context is not null && state is not null && inputEvent is TInputEvent typeedEvent)
        {
            Process(context, state, typeedEvent, deltaTime);
        }
    }

    #endregion

    #region Helpers

    protected abstract void Process(IUserInputContext context, IInputState state, TInputEvent inputEvent, TimeSpan deltaTime);

    #endregion
}
