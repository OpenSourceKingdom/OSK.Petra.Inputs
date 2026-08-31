using Moq;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Devices;

public class DeviceDescriptorExtensionsTests
{
    #region IsGeneric

    [Fact]
    public void IsGeneric_GenericName_ReturnsTrue()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, DeviceIdentities.GenericDeviceName));

        // Act/Assert
        Assert.True(mock.Object.IsGeneric());
    }

    [Fact]
    public void IsGeneric_GenericCaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "GENERIC"));

        // Act/Assert
        Assert.True(mock.Object.IsGeneric());
    }

    [Fact]
    public void IsGeneric_NonGenericName_ReturnsFalse()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Xbox Controller"));

        // Act/Assert
        Assert.False(mock.Object.IsGeneric());
    }

    [Fact]
    public void IsGeneric_GenericFamily_NonGenericName_ReturnsFalse()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Xbox Controller"));

        // Act/Assert
        Assert.False(mock.Object.IsGeneric());
    }

    #endregion
}
