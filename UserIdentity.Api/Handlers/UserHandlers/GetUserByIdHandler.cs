using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Proto.User;
using UserIdentity.Api.Extensions;
using UserIdentity.Api.Queries;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Handlers.UserHandlers
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IValidator<GetUserByIdRequest> _validator;

        public GetUserByIdHandler(UserManager<User> userManager, IMapper mapper,
            IValidator<GetUserByIdRequest> validator)
        {
            _userManager = userManager;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(query.Id);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return await _userManager.CollectUserDataAsync(user, _mapper);
        }
    }
}