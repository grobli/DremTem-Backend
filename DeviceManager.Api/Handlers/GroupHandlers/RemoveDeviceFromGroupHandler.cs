using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.GroupHandlers
{
    public class RemoveDeviceFromGroupHandler : IRequestHandler<RemoveDeviceFromGroupCommand, Empty>
    {
        private readonly IGroupService _groupService;
        private readonly IDeviceService _deviceService;
        private readonly IValidator<RemoveDeviceFromGroupRequest> _validator;

        public RemoveDeviceFromGroupHandler(IGroupService groupService,
            IValidator<RemoveDeviceFromGroupRequest> validator,
            IDeviceService deviceService)
        {
            _groupService = groupService;
            _validator = validator;
            _deviceService = deviceService;
        }

        public async Task<Empty> Handle(RemoveDeviceFromGroupCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = command.Body.UserId();

            var groupQuery = _groupService.GetGroupQuery(command.Body.GroupId, userId).Include(g => g.Devices);
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

            if (!group.Devices.Contains(device))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Device is not member of the group"));
            }

            await _groupService.RemoveDevice(group, device, cancellationToken);
            return new Empty();
        }
    }
}