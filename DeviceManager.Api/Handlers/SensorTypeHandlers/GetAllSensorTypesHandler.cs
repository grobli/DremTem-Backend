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
using Shared;
using Shared.Extensions;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.SensorType;

namespace DeviceManager.Api.Handlers.SensorTypeHandlers
{
    public class GetAllSensorTypesHandler : IRequestHandler<GetAllSensorTypesQuery, GetAllSensorTypesResponse>
    {
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetManyRequest> _validator;

        public GetAllSensorTypesHandler(ISensorTypeService typeService, IMapper mapper,
            IValidator<GenericGetManyRequest> validator)
        {
            _typeService = typeService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<GetAllSensorTypesResponse> Handle(GetAllSensorTypesQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.QueryParameter;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var types = _typeService.GetAllSensorTypesQuery();
            var pagedList = await PagedList<SensorType>.ToPagedListAsync(types, query.PageNumber, query.PageSize,
                cancellationToken);

            var response = new GetAllSensorTypesResponse()
            {
                SensorTypes =
                {
                    pagedList.Select(t => _mapper.Map<SensorType, SensorTypeDto>(t))
                },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };
            return response;
        }
    }
}