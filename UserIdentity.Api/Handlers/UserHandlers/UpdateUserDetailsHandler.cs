using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Proto.User;
using UserIdentity.Api.Commands;
using UserIdentity.Api.Extensions;
using UserIdentity.Core.Messages;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Handlers.UserHandlers
{
    public class UpdateUserDetailsHandler : IRequestHandler<UpdateUserDetailsCommand, UserDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateUserDetailsRequest> _validator;
        private readonly IBus _bus;

        public UpdateUserDetailsHandler(UserManager<User> userManager, IMapper mapper,
            IValidator<UpdateUserDetailsRequest> validator, IBus bus)
        {
            _userManager = userManager;
            _mapper = mapper;
            _validator = validator;
            _bus = bus;
        }

        public async Task<UserDto> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(request.Body.Id);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            user.FirstName = request.Body.FirstName;
            user.LastName = request.Body.LastName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new RpcException(new Status(StatusCode.Internal, updateResult.Errors.First().ToString()));

            var userDto = await _userManager.CollectUserDataAsync(user, _mapper);
            var message = new UpdatedUserDetailsMessage(userDto);
            await _bus.PubSub.PublishAsync(message, cancellationToken);

            return userDto;
        }
    }
}