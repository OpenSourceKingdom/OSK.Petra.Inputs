using OSK.Petra.Inputs.Abstractions.Inputs;
using Moq;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Inputs;

public class DeviceDescriptorExtensionsTests
{
    #region IsGeneric

    [Fact]
    public void IsGeneric_GenericName_ReturnsTrue()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, DeviceNames.Generic));

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
