using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared;
using Shared.Extensions;
using Shared.Proto;
using UserIdentity.Api.Queries;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Handlers.UserHandlers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>
    {
        private readonly IValidator<GetAllUsersRequest> _validator;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetAllUsersHandler(IValidator<GetAllUsersRequest> validator, UserManager<User> userManager,
            IMapper mapper)
        {
            _validator = validator;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<GetAllUsersResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var users = _userManager.Users.OrderBy(u => u.UserName);
            var pagedList = await PagedList<User>.ToPagedListAsync(users, query.PageNumber,
                query.PageSize, cancellationToken);

            var response = new GetAllUsersResponse
            {
                Users = { pagedList.Select(u => _mapper.Map<User, UserDto>(u)) },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;
        }
    }
}