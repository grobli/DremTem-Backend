using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Proto;
using Shared.Extensions;

namespace DeviceManager.Api.Handlers.GroupHandlers
{
    public class AddDeviceToGroupHandler : IRequestHandler<AddDeviceToGroupCommand, Empty>
    {
        private readonly IGroupService _groupService;
        private readonly IDeviceService _deviceService;
        private readonly IValidator<AddDeviceToGroupRequest> _validator;

        public AddDeviceToGroupHandler(IGroupService groupService, IValidator<AddDeviceToGroupRequest> validator,
            IDeviceService deviceService)
        {
            _groupService = groupService;
            _validator = validator;
            _deviceService = deviceService;
        }

        public async Task<Empty> Handle(AddDeviceToGroupCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = command.Body.UserId();

            var groupQuery = _groupService.GetGroupQuery(command.Body.GroupId, userId);
            var group = await groupQuery.SingleOrDefaultAsync(cancellationToken);
            if (group is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Group not found"));
            }

            var deviceQuery = _deviceService.GetDeviceQuery(command.Body.DeviceId, userId);
            var device = await deviceQuery.SingleOrDefaultAsync(cancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Device not found"));
            }

            await _groupService.AddDevice(group, device, cancellationToken);
            return new Empty();
        }
    }
}