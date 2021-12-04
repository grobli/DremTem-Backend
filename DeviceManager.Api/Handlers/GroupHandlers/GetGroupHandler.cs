using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.GroupHandlers
{
    public class GetGroupHandler : IRequestHandler<GetGroupQuery, GroupDto>
    {
        private readonly IValidator<GenericGetRequest> _validator;
        private readonly IMapper _mapper;
        private readonly IGroupService _groupService;

        public GetGroupHandler(IValidator<GenericGetRequest> validator, IMapper mapper, IGroupService groupService)
        {
            _validator = validator;
            _mapper = mapper;
            _groupService = groupService;
        }

        public async Task<GroupDto> Handle(GetGroupQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            var groupQuery = _groupService.GetGroupQuery(query.Id, userId).Include(g => g.Devices);


            var group = await groupQuery.SingleOrDefaultAsync(cancellationToken);
            if (group is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            var deviceMap = _mapper.Map<Group, GroupDto>(group);
            return deviceMap;
        }
    }
}