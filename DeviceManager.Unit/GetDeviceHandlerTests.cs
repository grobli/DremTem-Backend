using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Handlers.DeviceHandlers;
using DeviceManager.Api.Mapping;
using DeviceManager.Api.Queries;
using DeviceManager.Api.Validation;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using FluentAssertions;
using Grpc.Core;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;
using Shared.Proto;
using Xunit;

namespace DeviceManager.Unit
{
    public class GetDeviceHandlerTests
    {
        private readonly GetDeviceHandler _sut; // system under tests
        private readonly Mock<IDeviceService> _deviceServiceMock = new();
        private readonly Mock<ISensorService> _sensorServiceMock = new();

        public GetDeviceHandlerTests()
        {
            var validator = new GenericGetRequestValidator();
            var mapper = new Mapper(new MapperConfiguration(
                expression => expression.AddProfile<MappingProfile>()));
            _sut = new GetDeviceHandler(validator, mapper, _deviceServiceMock.Object, _sensorServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDevice_WhenDeviceExists()
        {
            // Arrange
            const int deviceId = 42;
            const string deviceName = "Device";
            var device = new Device { Id = deviceId, Name = deviceName, UserId = Guid.Empty };

            var request = new GenericGetRequest { Id = deviceId };
            var query = new GetDeviceQuery(request);

            var devices = new List<Device> { device }.AsQueryable().BuildMock();
            _deviceServiceMock.Setup(x => x.GetDeviceQuery(deviceId, Guid.Empty))
                .Returns(devices.Object);

            var sensors = new List<Sensor>().AsQueryable().BuildMock();
            _sensorServiceMock.Setup(x => x.GetAllSensorsQuery(Guid.Empty))
                .Returns(sensors.Object);

            // Act
            var deviceDto = await _sut.Handle(query, CancellationToken.None);

            // Assert
            deviceDto.Id.Should().Be(deviceId);
            deviceDto.Name.Should().Be(deviceName);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDeviceNotExist()
        {
            // Arrange
            const int deviceId = 42;
            var request = new GenericGetRequest { Id = deviceId };
            var query = new GetDeviceQuery(request);

            var devices = new List<Device>().AsQueryable().BuildMock();
            _deviceServiceMock.Setup(x => x.GetDeviceQuery(It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns(devices.Object);

            // Act
            Func<Task> act = async () => await _sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<RpcException>()
                .Where(e => e.Status.StatusCode.Equals(StatusCode.NotFound));
        }
    }
}