using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;

namespace DeviceManager.Api.Handlers.GroupHandlers
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, GroupDto>
    {
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateGroupRequest> _validator;

        public CreateGroupHandler(IGroupService groupService, IMapper mapper, IValidator<CreateGroupRequest> validator)
        {
            _groupService = groupService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newGroup = _mapper.Map<CreateGroupRequest, Group>(request.Body);

            try
            {
                var createdGroup = await _groupService.CreateGroupAsync(newGroup, cancellationToken);
                return _mapper.Map<Group, GroupDto>(createdGroup);
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}