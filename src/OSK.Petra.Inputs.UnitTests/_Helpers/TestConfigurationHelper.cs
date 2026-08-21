using Moq;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.UnitTests._Helpers;

public static class TestConfigurationHelper
{
    public static readonly DeviceIdentity KeyboardIdentity = new(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");
    public static readonly DeviceIdentity MouseIdentity = new(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse");

    public static InputSystemConfiguration CreateValidConfiguration() => CreateValidConfiguration(4);

    public static InputSystemConfiguration CreateValidConfiguration(int maxUsers, bool markSchemeAsCustom = false)
    {
        var actions = new[]
        {
            new InputAction("Move", new HashSet<InputPhase> { InputPhase.Start }, ctx => { }, "Moves the cursor"),
            new InputAction("Click", new HashSet<InputPhase> { InputPhase.Start, InputPhase.End }, ctx => { }, "Clicks")
        };
        var definition = new ActionDefinition("Default", actions, isDefault: true);

        var deviceMaps = new List<DeviceInputMap>
        {
            new DeviceInputMap
            {
                DeviceIdentity = KeyboardIdentity,
                InputMaps = new[]
                {
                    new InputActionMap(actions[0], Mock.Of<IInput>())
                }
            },
            new DeviceInputMap
            {
                DeviceIdentity = MouseIdentity,
                InputMaps = new[]
                {
                    new InputActionMap(actions[1], Mock.Of<IInput>())
                }
            }
        };

        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: markSchemeAsCustom);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard, DeviceTopologyName.Mouse });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = maxUsers,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced
        };

        return new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy);
    }

    public static RuntimeDeviceIdentifier CreateDeviceIdentifier(DeviceTopologyName topology, int deviceId = 100)
    {
        return new RuntimeDeviceIdentifier(deviceId, new DeviceIdentity(topology, DeviceFamily.Generic, "TestDevice"));
    }
}
