using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserIdentity.Api.Extensions;
using UserIdentity.Api.Queries;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Handlers.UserHandlers
{
    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IValidator<GetUserByEmailRequest> _validator;

        public GetUserByEmailHandler(UserManager<User> userManager, IMapper mapper,
            IValidator<GetUserByEmailRequest> validator)
        {
            _userManager = userManager;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.NormalizedEmail == query.Email.ToUpper(),
                cancellationToken);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return await _userManager.CollectUserDataAsync(user, _mapper);
        }
    }
}