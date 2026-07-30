using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSK.DataStructures;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Models;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Internal.Models;

namespace OSK.Petra.Inputs.Internal.Services;

internal class SchemeEditor: ISchemeEditor
{
    #region Variables

    private readonly IInputUser _user;
    private readonly IInputConfigurationProvider _inputConfigurationProvider;
    private readonly SchemeService _schemeService;

    #endregion

    #region Constructors

    public SchemeEditor(IInputUser user, IInputConfigurationProvider inputConfigurationProvider, SchemeService schemeService) 
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _inputConfigurationProvider = inputConfigurationProvider ?? throw new ArgumentNullException(nameof(inputConfigurationProvider));
        _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));

        UpdateEditableScheme();
    }

    #endregion

    #region ISchemeEditor

    public IEditableInputScheme EditableScheme { get; private set; }

    public bool AllowCustomScheme { get; }

    public ICollectionNavigator<InputConfiguration> InputConfigurationNavigator { get; private set; }

    public ICollectionNavigator<ActionDefinition> DefinitionNavigator { get; private set; }

    public ICollectionNavigator<InputScheme> SchemeNavigator { get; private set; }

    public Output CreateNewScheme(int? deviceCombinationId = null)
    {
        if (!AllowCustomScheme)
        {
            return Out.InvalidRequest("Unable to create an input scheme as it is not supported.");
        }
        if (CurrentScheme is not null)
        {
            CurrentSchemeId += 1;
        }

        UpdateEditableScheme();
        return Out.Created();
    }

    public async Task<Output> DeleteSchemeAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentScheme is null)
        {
            CurrentSchemeId -= 1;
            SetScheme(CurrentSchemeId);
            return Out.Success();
        }

        throw new NotImplementedException();
    }

    public Task<Output> SaveSchemeAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Helpers

    private int FindInitialInputDefinitionIndex(IReadOnlyCollection<ActionDefinition> definitions, IInputUser user)
    {
        var indexDefinitionPair = definitions.Select((definition, index) => new { Definition = definition, Index = index })
                                             .FirstOrDefault(pair => pair.Definition.Name.Equals(user.ActiveInputDefinitionName, StringComparison.OrdinalIgnoreCase));

        indexDefinitionPair ??= definitions.Select((definition, index) => new { Definition = definition, Index = index }).FirstOrDefault(definition => definition.Definition.IsDefault);
        return indexDefinitionPair?.Index ?? 0;
    }

    private int FindInitialInputSchemeIndex(IReadOnlyCollection<InputScheme> schemes, IInputUser user)
    {
        var indexSchemePair = user.ActiveScheme.HasValue
                ? schemes.Select((scheme, index) => new { Scheme = scheme, Index = index }).FirstOrDefault(pair => pair.Scheme.Name.Equals(user.ActiveScheme.Value.SchemeName, StringComparison.OrdinalIgnoreCase))
                : schemes.Select((scheme, index) => new { Scheme = scheme, Index = index }).FirstOrDefault(pair => pair.Scheme.IsDefault);

        return indexSchemePair?.Index ?? 0;
    }

    private int FindInitialDeviceCombinationIndex(IReadOnlyList<InputConfiguration> deviceCombinations, IInputUser user)
    {
        return user.ActiveScheme.HasValue
                ? deviceCombinations.Select((combination, index) => new { DeviceCombination = combination, Index = index }).FirstOrDefault(pair => pair.DeviceCombination.Id.Equals(user.ActiveScheme.Value.DeviceCombinationId))?.Index ?? 0
                : 0;
    }

    [MemberNotNull(nameof(EditableScheme))]
    private void UpdateEditableScheme()
    {
        var preferredScheme = _user.GetPreferredInputScheme(, CurrentTopologyGroup.Id);
        var name = CurrentScheme is null
            ? "New Scheme"
            : CurrentScheme.Name;
        var readOnly = !CurrentScheme?.IsCustom ?? false;
        var isPreferred = CurrentScheme is not null && preferredScheme.HasValue && preferredScheme.Value.SchemeName.Equals(CurrentScheme.Name, StringComparison.OrdinalIgnoreCase);

        // TODO: Get the data
        EditableScheme = new EditableScheme(name, readOnly, isPreferred, CurrentDefinition.Actions, [], []);
    }

    #endregion
}
