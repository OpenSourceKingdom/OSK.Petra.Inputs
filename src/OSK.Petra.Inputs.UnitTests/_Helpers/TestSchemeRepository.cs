using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.UnitTests._Helpers;

public class TestSchemeRepository : ISchemeRepository
{
    public bool AllowCustomSchemes => throw new NotImplementedException();

    public Task<Output> DeleteCustomSchemeAsync(string definitionName, string schemeName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Output<IEnumerable<CustomInputScheme>>> GetCustomSchemesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Output<IEnumerable<PreferredInputScheme>>> GetPreferredSchemesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Output<CustomInputScheme>> SaveCustomInputScheme(CustomInputScheme scheme, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Output<PreferredInputScheme>> SavePreferredSchemeAsync(PreferredInputScheme scheme, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
