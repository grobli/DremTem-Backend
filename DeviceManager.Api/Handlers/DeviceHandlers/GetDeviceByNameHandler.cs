using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Device;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class GetDeviceByNameHandler : IRequestHandler<GetDeviceByNameQuery, DeviceExtendedDto>
    {
        private readonly IValidator<GetDeviceByNameRequest> _validator;
        private readonly IMapper _mapper;
        private readonly IDeviceService _deviceService;
        private readonly ISensorService _sensorService;

        public GetDeviceByNameHandler(IValidator<GetDeviceByNameRequest> validator, IMapper mapper,
            IDeviceService deviceService, ISensorService sensorService)
        {
            _validator = validator;
            _mapper = mapper;
            _deviceService = deviceService;
            _sensorService = sensorService;
        }


        public async Task<DeviceExtendedDto> Handle(GetDeviceByNameQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            IQueryable<Device> deviceQuery = _deviceService.GetAllDevicesQuery(userId)
                .Where(d => d.Name == query.DeviceName).Include(d => d.Groups);
            if (query.Parameters != null)
                deviceQuery = query.Parameters.IncludeFieldsSet(Entity.Location, Entity.Sensor)
                    .Aggregate(deviceQuery, (current, field) => field switch
                    {
                        Entity.Location => current.Include(d => d.Location),
                        Entity.Sensor => current.Include(d => d.Sensors),
                        _ => current
                    });

            var device = await deviceQuery.SingleOrDefaultAsync(cancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            var deviceMap = _mapper.Map<Device, DeviceExtendedDto>(device);

            // add sensor ids
            var sensors = query.Parameters.IncludeFieldsSet(Entity.Sensor).Count > 0
                ? deviceMap.Sensors.Select(s => s.Id)
                : await _sensorService.GetAllSensorsQuery(userId)
                    .Where(s => s.DeviceId == device.Id)
                    .Select(s => s.Id)
                    .ToArrayAsync(cancellationToken);
            deviceMap.SensorIds.AddRange(sensors);

            return deviceMap;
        }
    }
}