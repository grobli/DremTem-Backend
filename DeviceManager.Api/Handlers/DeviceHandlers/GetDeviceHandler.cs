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

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class GetDeviceHandler : IRequestHandler<GetDeviceQuery, DeviceExtendedDto>
    {
        private readonly IValidator<GenericGetRequest> _validator;
        private readonly IMapper _mapper;
        private readonly IDeviceService _deviceService;

        public GetDeviceHandler(IValidator<GenericGetRequest> validator, IMapper mapper, IDeviceService deviceService)
        {
            _validator = validator;
            _mapper = mapper;
            _deviceService = deviceService;
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
            var deviceQuery = _deviceService.GetDeviceQuery(query.Id, userId);
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

            return _mapper.Map<Core.Models.Device, DeviceExtendedDto>(device);
        }
    }
}