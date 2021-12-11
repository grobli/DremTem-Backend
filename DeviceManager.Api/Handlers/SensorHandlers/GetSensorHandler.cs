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

namespace DeviceManager.Api.Handlers.SensorHandlers
{
    public class GetSensorHandler : IRequestHandler<GetSensorQuery, SensorDto>
    {
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetRequest> _validator;

        public GetSensorHandler(ISensorService sensorService, IMapper mapper, IValidator<GenericGetRequest> validator)
        {
            _sensorService = sensorService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<SensorDto> Handle(GetSensorQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            var sensorQuery = _sensorService.GetSensorQuery(query.Id, userId);
            if (query.Parameters != null &&
                query.Parameters.IncludeFieldsSet(Entity.SensorType).Contains(Entity.SensorType))
            {
                sensorQuery = sensorQuery.Include(l => l.Type);
            }

            var sensor = await sensorQuery.SingleOrDefaultAsync(cancellationToken);
            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            return await Task.FromResult(_mapper.Map<Sensor, SensorDto>(sensor));
        }
    }
}