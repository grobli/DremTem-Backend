using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Proto;
using UserIdentity.Api.Commands;
using UserIdentity.Core.Messages;

namespace UserIdentity.Api.Handlers.UserHandlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
    {
        private readonly UserManager<Core.Models.Auth.User> _userManager;
        private readonly IValidator<DeleteUserRequest> _validator;
        private readonly IBus _bus;

        public DeleteUserHandler(UserManager<Core.Models.Auth.User> userManager,
            IValidator<DeleteUserRequest> validator, IBus bus)
        {
            _userManager = userManager;
            _validator = validator;
            _bus = bus;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(request.Body.Id);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            if (user.UserName == "admin")
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied,
                    "Default admin account cannot be deleted!"));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new RpcException(new Status(StatusCode.Internal, result.Errors.First().ToString()));
            }

            var message = new DeletedUserMessage(userId);
            await _bus.PubSub.PublishAsync(message, cancellationToken);

            return new DeleteUserResponse { DeletedUserId = user.Id.ToString() };
        }
    }
}