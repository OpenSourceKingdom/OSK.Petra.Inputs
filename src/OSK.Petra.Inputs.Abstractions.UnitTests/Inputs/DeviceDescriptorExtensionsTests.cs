using OSK.Petra.Inputs.Abstractions.Inputs;
using Moq;

namespace OSK.Petra.Inputs.Abstractions.UnitTests;

public class DeviceDescriptorExtensionsTests
{
    #region IsGeneric

    [Fact]
    public void IsGeneric_GenericName_ReturnsTrue()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "Generic"));

        // Act
        var result = mock.Object.IsGeneric();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsGeneric_GenericCaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Generic, "GENERIC"));

        // Act
        var result = mock.Object.IsGeneric();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsGeneric_NonGenericName_ReturnsFalse()
    {
        // Arrange
        var mock = new Mock<IDeviceDescriptor>();
        mock.Setup(d => d.Identity).Returns(new DeviceIdentity(DeviceTopologyName.Keyboard, DeviceFamily.Xbox, "Xbox Controller"));

        // Act
        var result = mock.Object.IsGeneric();

        // Assert
        Assert.False(result);
    }

    #endregion
}
