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
using Shared;
using Shared.Extensions;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Device;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class GetAllDevicesHandler : IRequestHandler<GetAllDevicesQuery, GetAllDevicesResponse>
    {
        private readonly IValidator<GenericGetManyRequest> _validator;
        private readonly IDeviceService _deviceService;
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;

        public GetAllDevicesHandler(IValidator<GenericGetManyRequest> validator, IDeviceService deviceService,
            IMapper mapper, ISensorService sensorService)
        {
            _validator = validator;
            _deviceService = deviceService;
            _mapper = mapper;
            _sensorService = sensorService;
        }

        public async Task<GetAllDevicesResponse> Handle(GetAllDevicesQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            IQueryable<Device> devices = _deviceService.GetAllDevicesQuery(userId).Include(d => d.Groups);
            if (query.Parameters != null)
                devices = query.Parameters.IncludeFieldsSet(Entity.Location, Entity.Sensor)
                    .Aggregate(devices, (current, field) => field switch
                    {
                        Entity.Location => current.Include(d => d.Location),
                        Entity.Sensor => current.Include(d => d.Sensors),
                        _ => current
                    });

            var pagedList = await PagedList<Device>.ToPagedListAsync(devices, query.PageNumber,
                query.PageSize, cancellationToken);

            var pagedListMapped = pagedList
                .Select(d => _mapper.Map<Device, DeviceExtendedDto>(d))
                .ToList();

            await AddSensorIdsToMap();

            var response = new GetAllDevicesResponse
            {
                Devices = { pagedListMapped },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;

            async Task AddSensorIdsToMap()
            {
                // add sensor ids 
                if (query.Parameters.IncludeFieldsSet(Entity.Sensor).Count > 0)
                {
                    foreach (var device in pagedListMapped)
                    {
                        var sensors = device.Sensors.Select(s => s.Id);
                        device.SensorIds.AddRange(sensors);
                    }
                }
                else
                {
                    var deviceIds = pagedListMapped.Select(d => d.Id);
                    var sensors = await _sensorService.GetAllSensorsQuery(userId)
                        .Where(s => deviceIds.Contains(s.DeviceId))
                        .Select(s => new { s.DeviceId, s.Id })
                        .ToListAsync(cancellationToken);

                    var sensorDict = sensors
                        .GroupBy(s => s.DeviceId)
                        .Select(pair => new { DeviceId = pair.Key, SensorIds = pair.Select(p => p.Id) })
                        .ToDictionary(x => x.DeviceId, x => x.SensorIds);

                    foreach (var device in pagedListMapped)
                    {
                        device.SensorIds.AddRange(sensorDict[device.Id]);
                    }
                }
            }
        }
    }
}