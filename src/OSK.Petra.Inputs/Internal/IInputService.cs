using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Internal;

internal interface IInputService
{
    bool PauseInput { get; set; }

    void Update(TimeSpan delta);

    bool IsUserActionsSurpressed(int userId, int actionGroupId);

    Task<Output> InitializeAsync(CancellationToken cancellationToken = default);
}
