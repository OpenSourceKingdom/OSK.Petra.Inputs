using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;

namespace OSK.Petra.Inputs.Abstractions.Runtime;

/// <summary>
/// The active scheme that a user is using to interact with the input system.
/// See <see cref="InputScheme"/>
/// </summary>
/// <param name="definitionName">The name of the <see cref="ActionDefinition"/></param>
/// <param name="schemeName">The name of the <see cref="InputScheme"/></param>
/// <param name="deviceIdentities">The list of devices associated with the active scheme</param>
public readonly struct ActiveSchemeDetails(string definitionName, string schemeName, DeviceIdentity[] deviceIdentities)
{
    #region Variables

    public string DefinitionName => definitionName;

    public string SchemeName => schemeName;

    public string DeviceCombinationId { get; } = InputConfiguration.GetConfigurationId(deviceIdentities);

    public DeviceIdentity[] DeviceIdentities => deviceIdentities;

    #endregion
}
