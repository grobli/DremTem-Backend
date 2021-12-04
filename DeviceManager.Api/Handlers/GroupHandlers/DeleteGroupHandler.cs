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
    public class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, Empty>
    {
        private readonly IGroupService _groupService;
        private readonly IValidator<GenericDeleteRequest> _validator;

        public DeleteGroupHandler(IGroupService groupService, IValidator<GenericDeleteRequest> validator)
        {
            _groupService = groupService;
            _validator = validator;
        }

        public async Task<Empty> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var group = await _groupService.GetGroupQuery(request.Body.Id, request.Body.UserId())
                .SingleOrDefaultAsync(cancellationToken);
            if (group is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _groupService.DeleteGroupAsync(group, cancellationToken);
            return new Empty();
        }
    }
}