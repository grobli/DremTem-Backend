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
using DeviceManager.Core.Models;

namespace DeviceManager.Api.Handlers.GroupHandlers
{
    public class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, GroupDto>
    {
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateGroupRequest> _validator;

        public UpdateGroupHandler(IGroupService groupService, IMapper mapper,
            IValidator<UpdateGroupRequest> validator)
        {
            _groupService = groupService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<GroupDto> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
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

            try
            {
                await _groupService
                    .UpdateGroupAsync(group, _mapper.Map<UpdateGroupRequest, Group>(request.Body),
                        cancellationToken);
                return _mapper.Map<Group, GroupDto>(group);
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}