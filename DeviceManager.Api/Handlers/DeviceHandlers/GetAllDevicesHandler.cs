using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class GetAllDevicesHandler : IRequestHandler<GetAllDevicesQuery, GetAllDevicesResponse>
    {
        private readonly IValidator<GenericGetManyRequest> _validator;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;

        public GetAllDevicesHandler(IValidator<GenericGetManyRequest> validator, IDeviceService deviceService,
            IMapper mapper)
        {
            _validator = validator;
            _deviceService = deviceService;
            _mapper = mapper;
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
            var devices = _deviceService.GetAllDevicesQuery(userId);
            if (query.Parameters != null)
                devices = query.Parameters.IncludeFieldsSet(Entity.Location, Entity.Sensor)
                    .Aggregate(devices, (current, field) => field switch
                    {
                        Entity.Location => current.Include(d => d.Location),
                        Entity.Sensor => current.Include(d => d.Sensors),
                        _ => current
                    });

            var pagedList = await PagedList<Core.Models.Device>.ToPagedListAsync(devices, query.PageNumber,
                query.PageSize, cancellationToken);

            var response = new GetAllDevicesResponse
            {
                Devices = { pagedList.Select(d => _mapper.Map<Core.Models.Device, DeviceExtendedDto>(d)) },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;
        }
    }
}