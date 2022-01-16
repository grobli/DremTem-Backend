using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Proto;
using UserIdentity.Api.Commands;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Handlers.AuthHandlers
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Empty>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<ChangePasswordRequest> _validator;

        public ChangePasswordHandler(UserManager<User> userManager, IValidator<ChangePasswordRequest> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<Empty> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var user = _userManager.Users.SingleOrDefault(u => u.Id.ToString() == command.Body.UserId);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"User with id={command.Body.UserId} not found"));
            }

            var changePasswordResult =
                await _userManager.ChangePasswordAsync(user, command.Body.OldPassword, command.Body.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    changePasswordResult.Errors.First().Description));
            }

            return new Empty();
        }
    }
}