using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserIdentity.Api.Commands;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Handlers.AuthHandlers
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Empty>
    {
        private readonly RoleManager<Role> _roleManager;

        public CreateRoleHandler(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Empty> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Body.RoleName))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Role name should be provided"));
            }

            var newRole = new Role { Name = request.Body.RoleName };

            var roleResult = await _roleManager.CreateAsync(newRole);

            if (roleResult.Succeeded) return new Empty();

            throw new RpcException(new Status(StatusCode.Internal, roleResult.Errors.First().ToString()));
        }
    }
}