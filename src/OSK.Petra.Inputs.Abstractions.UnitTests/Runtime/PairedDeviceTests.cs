using OSK.Petra.Inputs.Abstractions.Runtime;

namespace OSK.Petra.Inputs.Abstractions.UnitTests.Runtime;

public class PairedDeviceTests
{
    #region Constructor

    [Fact]
    public void Constructor_SetsPropertiesToExpectedValues()
    {
        // Arrange & Act
        var identifier = new RuntimeDeviceIdentifier(99, default);
        var device = new PairedDevice(42, identifier);

        // Assert
        Assert.Equal(42, device.UserId);
        Assert.Equal(identifier, device.DeviceIdentifier);
    }

    #endregion

    #region Status

    [Fact]
    public void Status_DefaultValue_IsActive()
    {
        // Arrange & Act
        var device = new PairedDevice(1, new RuntimeDeviceIdentifier(1, default));

        // Assert
        Assert.Equal(DeviceStatus.Active, device.Status);
    }

    #endregion

    #region UpdateStatus

    [Fact]
    public void UpdateStatus_Disconnected_SetsStatusToDisconnected()
    {
        // Arrange
        var device = new PairedDevice(1, new RuntimeDeviceIdentifier(1, default));

        // Act
        device.UpdateStatus(DeviceStatus.Disconnected);

        // Assert
        Assert.Equal(DeviceStatus.Disconnected, device.Status);
    }

    [Fact]
    public void UpdateStatus_Active_SetsStatusToActive()
    {
        // Arrange
        var device = new PairedDevice(1, new RuntimeDeviceIdentifier(1, default));
        device.UpdateStatus(DeviceStatus.Disconnected);

        // Act
        device.UpdateStatus(DeviceStatus.Active);

        // Assert
        Assert.Equal(DeviceStatus.Active, device.Status);
    }

    #endregion
}
