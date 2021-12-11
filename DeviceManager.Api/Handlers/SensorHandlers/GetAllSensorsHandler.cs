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

namespace DeviceManager.Api.Handlers.SensorHandlers
{
    public class GetAllSensorsHandler : IRequestHandler<GetAllSensorsQuery, GetAllSensorsResponse>
    {
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetManyRequest> _validator;

        public GetAllSensorsHandler(ISensorService sensorService, IMapper mapper,
            IValidator<GenericGetManyRequest> validator)
        {
            _sensorService = sensorService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<GetAllSensorsResponse> Handle(GetAllSensorsQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            var sensors = _sensorService.GetAllSensorsQuery(userId);
            if (query.Parameters != null &&
                query.Parameters.IncludeFieldsSet(Entity.SensorType).Contains(Entity.SensorType))
            {
                sensors = sensors.Include(s => s.Type);
            }

            var pagedList = await PagedList<Sensor>.ToPagedListAsync(sensors, query.PageNumber, query.PageSize,
                cancellationToken);

            var response = new GetAllSensorsResponse
            {
                Sensors = { pagedList.Select(s => _mapper.Map<Sensor, SensorDto>(s)) },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;
        }
    }
}