using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Ports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Internal;

internal interface IInternalSchemeService: ISchemeService
{
    Task<Output> LoadSchemeConfigurationAsync(CancellationToken cancellationToken = default);
}
