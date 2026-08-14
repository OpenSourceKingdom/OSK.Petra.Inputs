using Moq;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Models;

namespace OSK.Petra.Inputs.UnitTests._Helpers;

public static class TestConfigurationHelper
{
    public static readonly DeviceIdentity KeyboardIdentity = new(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Keyboard");
    public static readonly DeviceIdentity MouseIdentity = new(DeviceTopologyName.Mouse, DeviceFamily.Generic, "Mouse");

    public static InputSystemConfiguration CreateValidConfiguration() => CreateValidConfiguration(4);

    public static InputSystemConfiguration CreateValidConfiguration(int maxUsers)
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

        var mockKeyboardTopology = new Mock<IDeviceTopology>();
        mockKeyboardTopology.Setup(m => m.IsCompatibleInput(It.IsAny<IInput>())).Returns(true);
        mockKeyboardTopology.SetupGet(m => m.Name).Returns(DeviceTopologyName.Keyboard);

        var mockMouseTopology = new Mock<IDeviceTopology>();
        mockMouseTopology.Setup(m => m.IsCompatibleInput(It.IsAny<IInput>())).Returns(true);
        mockMouseTopology.SetupGet(m => m.Name).Returns(DeviceTopologyName.Mouse);

        var scheme = new InputScheme("Default", "Default", deviceMaps, isDefault: true, isCustom: false);
        var config = new InputConfiguration(new[] { DeviceTopologyName.Keyboard, DeviceTopologyName.Mouse });
        config.AddScheme(scheme);

        var joinPolicy = new InputSystemJoinPolicy
        {
            MaxUsers = maxUsers,
            UserJoinBehavior = UserJoinBehavior.DeviceActivation,
            DeviceJoinBehavior = DevicePairingBehavior.Balanced
        };

        return new InputSystemConfiguration([mockKeyboardTopology.Object, mockMouseTopology.Object], new[] { config }, new[] { definition }, joinPolicy);
    }

    public static RuntimeDeviceIdentifier CreateDeviceIdentifier(DeviceTopologyName topology, int deviceId = 100)
    {
        return new RuntimeDeviceIdentifier(deviceId, new DeviceIdentity(topology, DeviceFamily.Generic, "TestDevice"));
    }
}
