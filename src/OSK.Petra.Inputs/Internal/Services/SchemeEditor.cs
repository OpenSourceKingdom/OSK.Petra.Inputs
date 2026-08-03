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
using OSK.Petra.Inputs.Abstractions.Inputs;
using System.Collections.Generic;

namespace OSK.Petra.Inputs.Internal.Services;

internal class SchemeEditor: ISchemeEditor
{
    #region Variables

    private readonly IInputUser _user;
    private readonly ISchemeService _schemeService;
    private readonly IInputSystemConfigurationProvider _inputConfigurationProvider;
    private readonly IDeviceCatalogProvider _deviceCatalogProvider;
    private SelectedScheme _selectedScheme;

    private Dictionary<DeviceTopologyName, IDeviceDescriptor> _deviceSchemeDescriptors = [];

    #endregion

    #region Constructors

    public SchemeEditor(IInputUser user, ISchemeService schemeService, IInputSystemConfigurationProvider inputConfigurationProvider, IDeviceCatalogProvider deviceCatalogProvider) 
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
        _inputConfigurationProvider = inputConfigurationProvider ?? throw new ArgumentNullException(nameof(inputConfigurationProvider));
        _deviceCatalogProvider = deviceCatalogProvider ?? throw new ArgumentNullException(nameof(deviceCatalogProvider));

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

    public DeviceCatalog GetDeviceRegistry(DeviceTopologyName topologyName)
        => _deviceCatalogProvider.GetCatalog(topologyName);

    public Output SetSchemeDevice(DeviceTopologyName topologyName, string deviceName)
    {
        var deviceCatalog = _deviceCatalogProvider.GetCatalog(topologyName);
        if (deviceCatalog.KnownDevices.Count is 0 && deviceCatalog.GenericDevice is null)
        {
            return Out.InvalidRequest($"The topology {topologyName} is not supported and can not be used for schemes.");
        }

        var deviceDescriptor = deviceCatalog.KnownDevices.FirstOrDefault(device => device.Identity.Name.Equals(deviceName));
        if (deviceDescriptor is null)
        {
            return Out.InvalidRequest($"The device name '{deviceName}' is not a supported device and can not be used for schemes.");
        }

        _deviceSchemeDescriptors[topologyName] = deviceDescriptor;
        UpdateSelectedScheme(isNew: _selectedScheme.IsNew);

        return Out.Success();
    }

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
        if (!SelectedScheme.IsReadonly)
        {        
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
        }

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

        UpdateNavigators();
        UpdateSelectedScheme(isNew: false); 
        TryPubishEditorEvent();

        return Out.Success();
    }

    #endregion

    #region Helpers

    internal Task<Output> InitializeAsync(CancellationToken cancellationToken = default)
        => _deviceCatalogProvider.InitializeAsync(cancellationToken);

    private void SetDefaultDeviceDescriptors(InputConfiguration? configuration)
    {
        if (configuration is null)
        {
            return;
        }

        _deviceSchemeDescriptors.Count();
        foreach (var topologyName in configuration.TopologyNames)
        {
            var genericDevice = _inputConfigurationProvider.Configuration.GetTopologyDescriptor(topologyName)?.CreateGeneric();
            if (genericDevice is not null)
            {
                _deviceSchemeDescriptors[topologyName] = genericDevice;
            }
        }
    }

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
            SetDefaultDeviceDescriptors(navigationEvent.Current);
            SetupDefinitionNavigator();

            TryPubishEditorEvent();
        };

        SetDefaultDeviceDescriptors(InputConfigurationNavigator.Current);
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

        var schemeName = isNew ? "New Scheme" : SchemeNavigator.Current.Name;
        var currentMaps = isNew
            ? []
            : SchemeNavigator.Current.DeviceMaps.Select(deviceMap => new DeviceMapPairing<InputActionMap>(deviceMap.DeviceIdentity, deviceMap.InputMaps));
        var availableInputs = _deviceSchemeDescriptors.Select(kvp => new DeviceMapPairing<IInput>(kvp.Value.Identity, kvp.Value.Inputs));

        _selectedScheme = new SelectedScheme(SchemeNavigator.Current.Name, SchemeNavigator.Current.IsCustom, isPreferred, isNew, DefinitionNavigator.Current.Actions, availableInputs, currentMaps);
    }

    private void TryPubishEditorEvent()
    {
        EditorUpdated?.Invoke();
    }

    #endregion
}
