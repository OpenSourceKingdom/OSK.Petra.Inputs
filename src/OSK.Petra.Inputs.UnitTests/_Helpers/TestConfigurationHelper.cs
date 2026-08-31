using Moq;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Ports;
using OSK.Petra.Inputs.Abstractions;

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
                    new InputActionMap(actions[0], 0)
                }
            },
            new DeviceInputMap
            {
                DeviceIdentity = MouseIdentity,
                InputMaps = new[]
                {
                    new InputActionMap(actions[1], 0)
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
            DevicePairingBehavior = DevicePairingBehavior.Balanced
        };

        return new InputSystemConfiguration(new[] { config }, new[] { definition }, joinPolicy, new InputCapabilityOptionConfiguration([]));
    }

    public static RuntimeDeviceIdentifier CreateDeviceIdentifier(DeviceTopologyName topology, int deviceId = 100)
    {
        return new RuntimeDeviceIdentifier(deviceId, new DeviceIdentity(topology, DeviceFamily.Generic, "TestDevice"));
    }

    public static ICapabilityOptions<TOptions> CreateOptions<TOptions>(TOptions? options = null)
        where TOptions : CapabilityOptions, new()
    {
        var mockOptions = new Mock<ICapabilityOptions<TOptions>>();
        mockOptions.Setup(m => m.Value)
            .Returns(options ?? new());

        return mockOptions.Object;
    }
}
