using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Proto;
using UserIdentity.Api.Commands;
using UserIdentity.Api.Services;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Handlers.AuthHandlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, UserLoginResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtService _jwt;

        public LoginHandler(UserManager<User> userManager, JwtService jwt)
        {
            _userManager = userManager;
            _jwt = jwt;
        }

        public async Task<UserLoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == request.Body.Email,
                cancellationToken);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            var userIsLockedOut = await _userManager.IsLockedOutAsync(user);
            if (userIsLockedOut)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated,
                    $"User has been locked out due to exceeded limit of login attempts until: {user.LockoutEnd}"));
            }

            var userSignInResult = await _userManager.CheckPasswordAsync(user, request.Body.Password);
            if (!userSignInResult)
            {
                await _userManager.AccessFailedAsync(user);
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Email or password incorrect"));
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserLoginResponse { JwtToken = _jwt.GenerateJwt(user, roles) };
        }
    }
}