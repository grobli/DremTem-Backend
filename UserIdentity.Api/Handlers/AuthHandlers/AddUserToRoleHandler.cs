using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserIdentity.Api.Commands;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Handlers.AuthHandlers
{
    public class AddUserToRoleHandler : IRequestHandler<AddUserToRoleCommand, Empty>
    {
        private readonly UserManager<User> _userManager;

        public AddUserToRoleHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Empty> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == request.Body.Email,
                cancellationToken);
            var result = await _userManager.AddToRoleAsync(user, request.Body.RoleName);

            if (result.Succeeded) return new Empty();
            throw new RpcException(new Status(StatusCode.Internal, result.Errors.First().ToString()));
        }
    }
}