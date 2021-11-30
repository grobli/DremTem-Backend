using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserIdentity.Api.Commands;
using UserIdentity.Core.Messages;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Handlers.AuthHandlers
{
    public class SignUpHandler : IRequestHandler<SignUpCommand, Empty>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IValidator<UserSignUpRequest> _validator;
        private readonly IBus _bus;

        public SignUpHandler(UserManager<User> userManager, IMapper mapper, IValidator<UserSignUpRequest> validator,
            IBus bus)
        {
            _userManager = userManager;
            _mapper = mapper;
            _validator = validator;
            _bus = bus;
        }

        public async Task<Empty> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var user = _mapper.Map<UserSignUpRequest, User>(request.Body);

            var userCreateResult = await _userManager.CreateAsync(user, request.Body.Password);
            if (!userCreateResult.Succeeded)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, userCreateResult.Errors.First().Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, DefaultRoles.BaseUser);
            if (!roleResult.Succeeded)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, roleResult.Errors.First().Description));
            }

            var userDto = _mapper.Map<User, UserDto>(user);
            userDto.Roles.Add(DefaultRoles.BaseUser);
            var message = new CreatedUserMessage(userDto);
            await _bus.PubSub.PublishAsync(message, cancellationToken);

            return new Empty();
        }
    }
}