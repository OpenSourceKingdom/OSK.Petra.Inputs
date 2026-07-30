using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Inputs.Ports;

public interface ISchemeService
{
    bool AllowCustomSchemes { get; }

    Task<Output> SavePreferredSchemeAsync(PreferredInputScheme preferredScheme, CancellationToken cancellationToken = default);

    Task<Output> LoadConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a scheme editor that targets the provided user
    /// </summary>
    /// <param name="userId">The id of the user to target</param>
    /// <returns>A scheme editor that is targeted to the user id</returns>
    ISchemeEditor? GetSchemeEditor(int userId);
}
