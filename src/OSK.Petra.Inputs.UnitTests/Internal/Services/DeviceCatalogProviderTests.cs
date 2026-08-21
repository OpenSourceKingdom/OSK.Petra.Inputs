using Moq;
using OSK.Operations.Outputs;
using OSK.Petra.Inputs.Abstractions.Inputs;
using OSK.Petra.Inputs.Internal.Services;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class DeviceCatalogProviderTests
{
    #region Variables

    private readonly List<IDeviceProvider> _deviceProviders = [];
    private readonly DeviceCatalogProvider _provider;

    #endregion

    #region Constructors

    public DeviceCatalogProviderTests()
    {
        _provider = new DeviceCatalogProvider(_deviceProviders);
    }

    #endregion

    #region GetCatalogAsync

    [Fact]
    public async Task GetCatalogAsync_FirstCall_LoadsCatalog()
    {
        // Arrange
        var mockProvider = new Mock<IDeviceProvider>();
        mockProvider.Setup(p => p.GetDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success((IEnumerable<IDeviceDescriptor>)[]));

        _deviceProviders.Add(mockProvider.Object);

        // Act
        var result = await _provider.GetCatalogAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetCatalogAsync_SecondCall_ReturnsCached()
    {
        // Arrange
        var mockProvider = new Mock<IDeviceProvider>();
        mockProvider.Setup(p => p.GetDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success((IEnumerable<IDeviceDescriptor>)[]));

        _deviceProviders.Add(mockProvider.Object);

        // Act
        var result1 = await _provider.GetCatalogAsync(TestContext.Current.CancellationToken);
        var result2 = await _provider.GetCatalogAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(result1.Data, result2.Data);
        mockProvider.Verify(m => m.GetDevicesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCatalogAsync_DeviceProviderFails_ReturnsError()
    {
        // Arrange
        var mockProvider = new Mock<IDeviceProvider>();
        mockProvider.Setup(p => p.GetDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.InvalidRequest<IEnumerable<IDeviceDescriptor>>("provider error"));

        _deviceProviders.Add(mockProvider.Object);

        // Act
        var result = await _provider.GetCatalogAsync(TestContext.Current.CancellationToken);

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
        mockProvider.Setup(p => p.GetDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success((IEnumerable<IDeviceDescriptor>)[mockDescriptor1.Object, mockDescriptor2.Object]));

        _deviceProviders.Add(mockProvider.Object);

        // Act
        var result = await _provider.GetCatalogAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);

        Assert.Single(result.Data.Pages);
        Assert.Equal(2, result.Data.Pages[0].Devices.Count);
    }

    #endregion
}
