using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ClientApiGateway.Api.Resources;
using FluentAssertions;
using Shared.Proto;
using Xunit;

namespace ClientApiGateway.IntegrationTests
{
    public class DevicesControllerTests : IntegrationTest
    {
        [Fact]
        public async Task CreateDevice_WithValidDeviceData_ReturnsCreatedDevice()
        {
            // Arrange 
            var sensors = new List<CreateDeviceSensorResource>
            {
                new() { Name = "temp1", TypeId = 1 }
            };
            const string deviceName = "testDevice";
            var device = new CreateDeviceResource(deviceName, null, true, null, null, null, null, sensors);
            await AuthenticateAsync();

            // Act
            var response = await TestClient.PostAsJsonAsync($"{BaseUrl}/api/v1/devices", device);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var returnedDevice = await response.Content.ReadAsAsync<DeviceResource>();
            returnedDevice.Id.Should().NotBe(0);
            returnedDevice.Name.Should().Be(deviceName);
            returnedDevice.Sensors.Should().Contain(resource => resource.Name.Equals("temp1") && resource.TypeId == 1);
        }

        [Fact]
        public async Task GetAllDevices_WhenNoDevicesExists_ShouldReturnEmptyList()
        {
            // Arrange 
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync($"{BaseUrl}/api/v1/devices");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedDevices = await response.Content.ReadAsAsync<IEnumerable<DeviceResource>>();
            returnedDevices.Should().BeEmpty();
        }
    }
}