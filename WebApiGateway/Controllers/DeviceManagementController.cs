using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DeviceGrpcService.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using WebApiGateway.Models;

namespace WebApiGateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceManagementController : ControllerBase
    {
        private readonly ILogger<DeviceManagementController> _logger;
        private readonly Device.DeviceClient _deviceClient;
        private readonly IMapper _mapper;

        public DeviceManagementController(ILogger<DeviceManagementController> logger, Device.DeviceClient deviceClient,
            IMapper mapper)
        {
            _logger = logger;
            _deviceClient = deviceClient;
            _mapper = mapper;
        }

        #region DevicesCRUD

        // GET: api/v1/DeviceManagement/devices
        [HttpGet("devices", Name = "Get All Devices")]
        public async Task<IEnumerable<DeviceGrpcBaseModel>> GetAllDevices()
        {
            using var clientData = _deviceClient.GetAllDevices(new Empty());
            var devices = new List<DeviceGrpcBaseModel>(20);

            await foreach (var device in clientData.ResponseStream.ReadAllAsync())
            {
                devices.Add(device);
            }

            return devices;
        }

        // GET: api/v1/DeviceManagement/devices/nested
        [HttpGet("devices/nested", Name = "Get All Devices Nested")]
        public async Task<IEnumerable<DeviceGrpcNestedModel>> GetAllDevicesNested()
        {
            using var clientData = _deviceClient.GetAllDevicesNested(new Empty());
            var devices = new List<DeviceGrpcNestedModel>();

            await foreach (var device in clientData.ResponseStream.ReadAllAsync())
            {
                devices.Add(device);
            }

            return devices;
        }

        // GET: api/v1/DeviceManagement/devices/98fbabe3-3ff8-4b39-ad19-d44e7e2b6180
        [HttpGet("devices/{id:guid}", Name = "Get Device By ID")]
        public async Task<ActionResult<DeviceGrpcBaseModel>> GetDeviceById(Guid id)
        {
            try
            {
                var device = await _deviceClient.GetDeviceByIdAsync(new DeviceByIdRequest { ID = id.ToString() });
                return device;
            }
            catch (RpcException e)
            {
                return e.StatusCode switch
                {
                    Grpc.Core.StatusCode.NotFound => NotFound(new
                    {
                        e.StatusCode, Status = e.StatusCode.GetDisplayName(), e.Status.Detail,
                    }),
                    _ => Problem(e.Message, statusCode: (int)e.StatusCode)
                };
            }
        }

        // GET: api/v1/DeviceManagement/devices/nested/98fbabe3-3ff8-4b39-ad19-d44e7e2b6180
        [HttpGet("devices/nested/{id:guid}", Name = "Get Device By ID Nested")]
        public async Task<ActionResult<DeviceGrpcNestedModel>> GetDeviceByIdNested(Guid id)
        {
            try
            {
                var device = await _deviceClient.GetDeviceByIdNestedAsync(new DeviceByIdRequest { ID = id.ToString() });
                return device;
            }
            catch (RpcException e)
            {
                return e.StatusCode switch
                {
                    Grpc.Core.StatusCode.NotFound => NotFound(new
                    {
                        e.StatusCode, Status = e.StatusCode.GetDisplayName(), e.Status.Detail,
                    }),
                    _ => Problem(e.Message, statusCode: (int)e.StatusCode)
                };
            }
        }

        // POST: api/v1/DeviceManagement/devices
        [HttpPost("devices/")]
        public async Task<ActionResult<DeviceGrpcBaseModel>> CreateDevice(DeviceCreateDto deviceDto)
        {
            if (deviceDto.Online == null) return BadRequest("invalid model");

            var deviceCreateRequest = new DeviceCreateRequest { Online = (bool)deviceDto.Online };
            if (!string.IsNullOrEmpty(deviceDto.Name)) deviceCreateRequest.NameValue = deviceDto.Name;
            if (deviceDto.LocationID != null) deviceCreateRequest.LocationIdValue = (int)deviceDto.LocationID;

            try
            {
                var deviceCreated = await _deviceClient.CreateDeviceAsync(deviceCreateRequest);
                return new CreatedResult($"api/v1/devices/{deviceCreated.ID}", deviceCreated);
            }
            catch (RpcException e)
            {
                return BadRequest(JsonSerializer.Serialize(e.Status));
            }
        }

        // PUT: api/v1/DeviceManagement/devices/98fbabe3-3ff8-4b39-ad19-d44e7e2b6180
        [HttpPut("devices/{id:guid}")]
        public async Task<ActionResult<DeviceGrpcBaseModel>> UpdateDevice(Guid id, DeviceCreateDto deviceDto)
        {
            var deviceUpdateRequest = new DeviceUpdateRequest { ID = id.ToString() };
            if (!string.IsNullOrEmpty(deviceDto.Name)) deviceUpdateRequest.NameValue = deviceDto.Name;
            if (deviceDto.Online != null) deviceUpdateRequest.OnlineValue = (bool)deviceDto.Online;
            if (deviceDto.LocationID != null) deviceUpdateRequest.LocationIdValue = (int)deviceDto.LocationID;

            try
            {
                var updatedDevice = await _deviceClient.UpdateDeviceByIDAsync(deviceUpdateRequest);
                return updatedDevice;
            }
            catch (RpcException e)
            {
                return BadRequest(JsonSerializer.Serialize(e.Status));
            }
        }

        // DELETE: api/v1/DeviceManagement/devices/98fbabe3-3ff8-4b39-ad19-d44e7e2b6180
        [HttpDelete("devices/{id:guid}")]
        public async Task<ActionResult> DeleteDevice(Guid id)
        {
            try
            {
                await _deviceClient.DeleteDeviceByIDAsync(new DeviceByIdRequest { ID = id.ToString() });
                return new OkResult();
            }
            catch (RpcException e)
            {
                return BadRequest(JsonSerializer.Serialize(e.Status));
            }
        }

        #endregion
    }
}