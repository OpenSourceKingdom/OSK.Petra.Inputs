using OSK.Petra.Inputs.Abstractions.Inputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Inputs.Notifications;

internal class SchemeEditorInputCapturedNotification(int userId, DeviceIdentity deviceIdentity, IInput input): SchemeEditorNotification
{
    public int UserId => userId;

    public DeviceIdentity DeviceIdentity => deviceIdentity;

    public IInput Input => input;
}
