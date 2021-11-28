using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeviceManager.Core.Extensions;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class UpdateDeviceHandler : IRequestHandler<UpdateDeviceCommand, DeviceDto>
    {
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateDeviceRequest> _validator;

        public UpdateDeviceHandler(IDeviceService deviceService, IMapper mapper,
            IValidator<UpdateDeviceRequest> validator)
        {
            _deviceService = deviceService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<DeviceDto> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var device = await _deviceService.GetDeviceQuery(request.Body.Id, request.Body.UserId())
                .SingleOrDefaultAsync(cancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            try
            {
                await _deviceService
                    .UpdateDeviceAsync(device, _mapper.Map<UpdateDeviceRequest, Core.Models.Device>(request.Body),
                        cancellationToken);
                return _mapper.Map<Core.Models.Device, DeviceDto>(device);
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}