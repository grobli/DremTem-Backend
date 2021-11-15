using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserIdentity.Api.Validators;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Services.Rpc
{
    public class UserInfoService : UserInfoGrpc.UserInfoGrpcBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserInfoService> _logger;

        public UserInfoService(UserManager<User> userManager, ILogger<UserInfoService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override async Task<UserInfoResource> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id.ToString() == request.Id);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return await CollectUserDataAsync(user);
        }

        public override async Task<UserInfoResource> GetUserByEmail(GetUserByEmailRequest request,
            ServerCallContext context)
        {
            var validator = new GetUserByEmailValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ToString()));
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == request.Email);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return await CollectUserDataAsync(user);
        }

        public override async Task<UserInfoResource> UpdateUserFirstName(UpdateUserFirstNameRequest request,
            ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "First name cannot be empty"));
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id.ToString() == request.Id);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            user.FirstName = request.FirstName;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return await Task.FromResult(await CollectUserDataAsync(user));
            }

            throw new RpcException(new Status(StatusCode.Internal, result.Errors.First().ToString()));
        }

        public override async Task<UserInfoResource> UpdateUserLastName(UpdateUserLastNameRequest request,
            ServerCallContext context)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id.ToString() == request.Id);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            user.LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return await Task.FromResult(await CollectUserDataAsync(user));
            }

            throw new RpcException(new Status(StatusCode.Internal, result.Errors.First().ToString()));
        }

        private async Task<UserInfoResource> CollectUserDataAsync([NotNull] User user)
        {
            var userInfo = new UserInfoResource
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            var roles = await _userManager.GetRolesAsync(user);
            userInfo.Roles.Add(roles);

            return userInfo;
        }
    }
}