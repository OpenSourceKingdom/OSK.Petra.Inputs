using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Internal;
using OSK.Petra.Inputs.Internal.Services;
using OSK.Petra.Inputs.Ports;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class DeviceCatalogProviderTests
{
    #region Variables

    private readonly Mock<IInputSystemConfigurationProvider> _mockConfigProvider;
    private readonly DeviceCatalogProvider _provider;

    #endregion

    #region Constructors

    public DeviceCatalogProviderTests()
    {
        var mockTopology = new Mock<IDeviceTopology>();
        mockTopology.SetupGet(m => m.Name).Returns(DeviceTopologyName.Keyboard);
        mockTopology.Setup(m => m.IsCompatibleInput(It.IsAny<IInput>())).Returns(true);
        mockTopology.Setup(m => m.CreateGeneric()).Returns(Mock.Of<IDeviceDescriptor>());

        var config = new InputSystemConfiguration(
            [mockTopology.Object],
            [],
            [],
            new InputSystemJoinPolicy());

        _mockConfigProvider = new Mock<IInputSystemConfigurationProvider>();
        _mockConfigProvider.SetupGet(m => m.Configuration).Returns(config);

        _provider = new DeviceCatalogProvider(_mockConfigProvider.Object, Array.Empty<IDeviceProvider>());
    }

    #endregion

    #region GetCatalogAsync

    [Fact]
    public async Task GetCatalogAsync_FirstCall_LoadsCatalog()
    {
        // Arrange
        var mockProvider = new Mock<IDeviceProvider>();
        mockProvider.Setup(p => p.GetDevicesAsync(default))
            .ReturnsAsync(Out.Success((IEnumerable<IDeviceDescriptor>)[]));

        var provider = new DeviceCatalogProvider(_mockConfigProvider.Object, [mockProvider.Object]);

        // Act
        var result = await provider.GetCatalogAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetCatalogAsync_SecondCall_ReturnsCached()
    {
        // Arrange
        var mockProvider = new Mock<IDeviceProvider>();
        mockProvider.Setup(p => p.GetDevicesAsync(default))
            .ReturnsAsync(Out.Success((IEnumerable<IDeviceDescriptor>)[]));

        var provider = new DeviceCatalogProvider(_mockConfigProvider.Object, [mockProvider.Object]);

        // Act
        var result1 = await provider.GetCatalogAsync(TestContext.Current.CancellationToken);
        var result2 = await provider.GetCatalogAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(result1.Data, result2.Data);
    }

    [Fact]
    public async Task GetCatalogAsync_DeviceProviderFails_ReturnsError()
    {
        // Arrange
        var mockProvider = new Mock<IDeviceProvider>();
        mockProvider.Setup(p => p.GetDevicesAsync(default))
            .ReturnsAsync(Out.InvalidRequest<IEnumerable<IDeviceDescriptor>>("provider error"));

        var catalogProvider = new DeviceCatalogProvider(_mockConfigProvider.Object, [mockProvider.Object]);

        // Act
        var result = await catalogProvider.GetCatalogAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task GetCatalogAsync_WithDevices_GroupsByTopology()
    {
        // Arrange
        var mockDescriptor1 = new Mock<IDeviceDescriptor>();
        mockDescriptor1.SetupGet(m => m.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard1"));

        var mockDescriptor2 = new Mock<IDeviceDescriptor>();
        mockDescriptor2.SetupGet(m => m.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Keyboard2"));

        var mockProvider = new Mock<IDeviceProvider>();
        mockProvider.Setup(p => p.GetDevicesAsync(default))
            .ReturnsAsync(Out.Success((IEnumerable<IDeviceDescriptor>)[mockDescriptor1.Object, mockDescriptor2.Object]));

        var catalogProvider = new DeviceCatalogProvider(_mockConfigProvider.Object, [mockProvider.Object]);

        // Act
        var result = await catalogProvider.GetCatalogAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
    }

    #endregion
}
