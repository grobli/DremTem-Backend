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
using Shared.Proto;

namespace DeviceManager.Api.Handlers.SensorTypeHandlers
{
    public class GetSensorTypeHandler : IRequestHandler<GetSensorTypeQuery, SensorTypeDto>
    {
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetRequest> _validator;

        public GetSensorTypeHandler(ISensorTypeService typeService, IMapper mapper,
            IValidator<GenericGetRequest> validator)
        {
            _typeService = typeService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<SensorTypeDto> Handle(GetSensorTypeQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorTypeQuery(query.Id)
                .SingleOrDefaultAsync(cancellationToken);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeDto>(type));
        }
    }
}