using System;
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
using System.Diagnostics;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Internal.Services;

internal class SchemeEditor: ISchemeEditor
{
    #region Variables

    private readonly IInputUser _user;
    private readonly IInputConfigurationProvider _inputConfigurationProvider;
    private readonly SchemeService _schemeService;
    private SelectedScheme _selectedScheme;

    #endregion

    #region Constructors

    public SchemeEditor(IInputUser user, IInputConfigurationProvider inputConfigurationProvider, SchemeService schemeService) 
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _inputConfigurationProvider = inputConfigurationProvider ?? throw new ArgumentNullException(nameof(inputConfigurationProvider));
        _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));

        UpdateNavigators();
        UpdateSelectedScheme(isNew: false);
    }

    #endregion

    #region ISchemeEditor

    public event Action? EditorUpdated;

    public ISelectedScheme SelectedScheme => _selectedScheme;

    public bool AllowCustomScheme { get; }

    public ICollectionNavigator<InputConfiguration> InputConfigurationNavigator { get; private set; }

    public ICollectionNavigator<ActionDefinition> DefinitionNavigator { get; private set; }

    public ICollectionNavigator<InputScheme> SchemeNavigator { get; private set; }

    public Output CreateNewScheme(int? inputConfigurationId = null)
    {
        if (!AllowCustomScheme)
        {
            return Out.InvalidRequest("Unable to create an input scheme as it is not supported.");
        }

        UpdateSelectedScheme(isNew: true);
        return Out.Created();
    }

    public async Task<Output> DeleteSchemeAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedScheme.IsReadonly)
        {
            return Out.InvalidRequest($"Unable to delete the input scheme '{_selectedScheme.Name}', that is associated with action definition '{DefinitionNavigator.Current!.Name}' and input configuration '{InputConfigurationNavigator.Current!.GetDisplayName()}' because it is read-only.");
        }

        var deleteOutput = await _schemeService.DeleteCustomSchemeAsync(DefinitionNavigator.Current!.Name, SelectedScheme.Name, cancellationToken);
        if (!deleteOutput.IsSuccessful)
        {
            return deleteOutput;
        }

        UpdateNavigators();
        UpdateSelectedScheme(isNew: false); 
        TryPubishEditorEvent();

        return Out.Success();
    }

    public async Task<Output> SaveSchemeAsync(CancellationToken cancellationToken = default)
    {
        if (!AllowCustomScheme)
        {
            return Out.InvalidRequest("Unable to save a custom scheme since the backing repository does not allow it.");
        }
        if (SelectedScheme.IsReadonly)
        {
            if (_selectedScheme.InitiallyPreferred != _selectedScheme.IsPreferred)
            {
                return await _schemeService.SavePreferredSchemeAsync(new PreferredInputScheme()
                {
                    ConfigurationId = InputConfigurationNavigator.Current!.Id,
                    DefinitionName = DefinitionNavigator.Current!.Name,
                    SchemeName = _selectedScheme.Name,
                    UserId = _user.Id
                });
            }

            return Out.Success();
        }

        var scheme = new CustomInputScheme()
        {
            DefinitionName = DefinitionNavigator.Current!.Name,
            Name = SelectedScheme.Name,
            DeviceMaps = []
        };

        var saveOutput = await _schemeService.SaveCustomSchemeAsync(scheme, SchemeSavePermissions.Overwrite, cancellationToken);
        if (!saveOutput.IsSuccessful)
        {
            return saveOutput;
        }

        UpdateNavigators();
        UpdateSelectedScheme(isNew: false); 
        TryPubishEditorEvent();

        return Out.Success();
    }

    #endregion

    #region Helpers

    [MemberNotNull(nameof(InputConfigurationNavigator), nameof(DefinitionNavigator), nameof(SchemeNavigator))]
    private void UpdateNavigators()
    {
        SetupConfigurationNavigator();
        SetupDefinitionNavigator();
        SetupSchemeNavigator();
    }

    [MemberNotNull(nameof(InputConfigurationNavigator))]
    private void SetupConfigurationNavigator()
    {
        InputConfigurationNavigator = new CollectionNavigator<InputConfiguration>(_inputConfigurationProvider.Configuration.InputConfigurations, wrapNavigation: true);
        InputConfigurationNavigator.Navigated += navigationEvent =>
        {
            SetupDefinitionNavigator();

            TryPubishEditorEvent();
        };
    }

    [MemberNotNull(nameof(DefinitionNavigator))]
    private void SetupDefinitionNavigator()
    {
        DefinitionNavigator = new CollectionNavigator<ActionDefinition>(_inputConfigurationProvider.Configuration.Definitions, wrapNavigation: true);
        DefinitionNavigator.Navigated += _ =>
        {
            SchemeNavigator = new CollectionNavigator<InputScheme>(InputConfigurationNavigator.Current!.Schemes.Where(scheme => scheme.DefinitionName.Equals(DefinitionNavigator.Current!.Name)), wrapNavigation: true);
            SchemeNavigator.Navigated += _ =>

            UpdateSelectedScheme(isNew: false);

            TryPubishEditorEvent();
        };
    }

    [MemberNotNull(nameof(SchemeNavigator))]
    private void SetupSchemeNavigator()
    {
        SchemeNavigator = new CollectionNavigator<InputScheme>(InputConfigurationNavigator.Current!.Schemes.Where(scheme => scheme.DefinitionName.Equals(DefinitionNavigator.Current!.Name)), wrapNavigation: true);
        SchemeNavigator.Navigated += _ =>
        {
            UpdateSelectedScheme(isNew: false);
            TryPubishEditorEvent();
        };
    }

    [MemberNotNull(nameof(_selectedScheme))]
    private void UpdateSelectedScheme(bool isNew)
    {
        Debug.Assert(InputConfigurationNavigator.Current is not null);
        Debug.Assert(DefinitionNavigator.Current is not null);
        Debug.Assert(SchemeNavigator.Current is not null);

        var preferredScheme = _schemeService.GetPreferredInputScheme(_user.Id, InputConfigurationNavigator.Current.Id, DefinitionNavigator.Current.Name);
        var isPreferred = preferredScheme is not null && preferredScheme.Value.SchemeName.Equals(SchemeNavigator.Current.Name);

        _selectedScheme = new SelectedScheme(SchemeNavigator.Current.Name, SchemeNavigator.Current.IsCustom, isPreferred, DefinitionNavigator.Current.Actions, [], []);
    }

    private void TryPubishEditorEvent()
    {
        EditorUpdated?.Invoke();
    }

    #endregion
}
