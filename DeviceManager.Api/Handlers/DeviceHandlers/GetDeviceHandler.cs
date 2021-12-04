using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class GetDeviceHandler : IRequestHandler<GetDeviceQuery, DeviceExtendedDto>
    {
        private readonly IValidator<GenericGetRequest> _validator;
        private readonly IMapper _mapper;
        private readonly IDeviceService _deviceService;
        private readonly ISensorService _sensorService;

        public GetDeviceHandler(IValidator<GenericGetRequest> validator, IMapper mapper, IDeviceService deviceService,
            ISensorService sensorService)
        {
            _validator = validator;
            _mapper = mapper;
            _deviceService = deviceService;
            _sensorService = sensorService;
        }

        public async Task<DeviceExtendedDto> Handle(GetDeviceQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            IQueryable<Device> deviceQuery = _deviceService.GetDeviceQuery(query.Id, userId).Include(d => d.Groups);
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