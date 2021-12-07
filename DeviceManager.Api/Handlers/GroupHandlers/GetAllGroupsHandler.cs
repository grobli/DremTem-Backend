using System;
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
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Extensions;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Group;

namespace DeviceManager.Api.Handlers.GroupHandlers
{
    public class GetAllGroupsHandler : IRequestHandler<GetAllGroupsQuery, GetAllGroupsResponse>
    {
        private readonly IValidator<GenericGetManyRequest> _validator;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GetAllGroupsHandler(IValidator<GenericGetManyRequest> validator, IGroupService groupService,
            IMapper mapper)
        {
            _validator = validator;
            _groupService = groupService;
            _mapper = mapper;
        }

        public async Task<GetAllGroupsResponse> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            var groups = _groupService
                .GetAllGroupsQuery(userId)
                .Include(d => d.Devices);

            var pagedList = await PagedList<Group>.ToPagedListAsync(groups, query.PageNumber,
                query.PageSize, cancellationToken);

            var pagedListMapped = pagedList
                .Select(d => _mapper.Map<Group, GroupDto>(d))
                .ToList();

            var response = new GetAllGroupsResponse()
            {
                Groups = { pagedListMapped },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;
        }
    }
}