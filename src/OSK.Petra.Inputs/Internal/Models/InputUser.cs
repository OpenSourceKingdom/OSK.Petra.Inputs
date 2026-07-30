using System;
using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Inputs.Abstractions;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Internal.Models;

internal class InputUser(int id): IInputUser
{
    #region Variables

    private string _activeDefinitionName = string.Empty;
    private Dictionary<int, PairedDevice> _pairedDevices = [];
    private Dictionary<string, Dictionary<string, PreferredInputScheme>> _preferredSchemeLookup = [];

    #endregion

    #region Constructors

    internal InputUser(int id, Dictionary<int, PairedDevice> pairedDevices)
        : this(id)
    {
        _pairedDevices = pairedDevices;
    }

    #endregion

    #region IApplicationUser

    public int Id => id;

    public string ActiveInputDefinitionName 
    {
        get => _activeDefinitionName;
        internal set
        {
            _activeDefinitionName = value;

            // Resetting the active scheme since it relies on the definition name. It should be updated
            // once the input processor picks up the next input from the user.
            ActiveScheme = null;
        }
    }

    public ActiveInputScheme? ActiveScheme { get; internal set; }

    public PreferredInputScheme? GetPreferredInputScheme(string definitionName, string combinationId)
        => string.IsNullOrWhiteSpace(definitionName) || string.IsNullOrWhiteSpace(combinationId)
            || !(_preferredSchemeLookup.TryGetValue(definitionName, out var definitionSchemeLookup)
                && definitionSchemeLookup.TryGetValue(combinationId, out var scheme))
            ? null
            : scheme;

    public IReadOnlyCollection<PairedDevice> PairedDevices => _pairedDevices.Values;

    public PairedDevice? GetDevice(int deviceId)
        => _pairedDevices.TryGetValue(deviceId, out var device)
            ? device
            : null;

    #endregion

    #region Helpers

    public void SetPreferredSchemes(IEnumerable<PreferredInputScheme> preferredInputSchemes)
    {
        // Create our lookup using only one preferred scheme per definition per combination, if there are mulitples, we'll ignore them
        _preferredSchemeLookup = preferredInputSchemes.GroupBy(scheme
            => new { scheme.DefinitionName, scheme.ConfigurationId, scheme.SchemeName })
            .Select(schemeDuplicates => schemeDuplicates.First())
            .GroupBy(scheme => new { scheme.DefinitionName })
            .ToDictionary(schemeGroup => schemeGroup.Key.DefinitionName, 
                            schemeGroup => schemeGroup.ToDictionary(scheme => scheme.ConfigurationId, StringComparer.OrdinalIgnoreCase),
                            StringComparer.OrdinalIgnoreCase);
    }

    public void AddDevice(RuntimeDeviceIdentifier deviceIdentifier)
    {
        _pairedDevices[deviceIdentifier.DeviceId] = new PairedDevice(Id, deviceIdentifier);
    }

    public PairedDevice? RemoveDevice(int deviceId)
    {
        if (_pairedDevices.TryGetValue(deviceId, out var device))
        {
            _pairedDevices.Remove(deviceId);
            return device;
        }

        return null;
    }

    public IReadOnlyCollection<PairedDevice> GetPairedDevices()
        => _pairedDevices.Values;

    public PairedDevice? GetPairedDevice(int id)
        => _pairedDevices.TryGetValue(id, out var device)
            ? device
            : null;

    #endregion
}
