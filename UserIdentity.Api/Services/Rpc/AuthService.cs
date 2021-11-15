using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserIdentity.Api.Validators;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Services.Rpc
{
    public class AuthService : UserAuthGrpc.UserAuthGrpcBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly JwtService _jwt;


        public AuthService(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper,
            JwtService jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwt = jwt;
        }

        public override async Task<Empty> SignUp(UserSignUpRequest request, ServerCallContext context)
        {
            var validator = new UserSignUpRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ToString()));
            }

            var user = _mapper.Map<UserSignUpRequest, User>(request);

            var userCreateResult = await _userManager.CreateAsync(user, request.Password);

            if (userCreateResult.Succeeded)
            {
                return await Task.FromResult(new Empty());
            }

            throw new RpcException(new Status(StatusCode.Internal, userCreateResult.Errors.First().Description));
        }

        public override async Task<UserLoginResponse> SignIn(UserLoginRequest request, ServerCallContext context)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == request.Email);
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

            var userSignInResult = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!userSignInResult)
            {
                await _userManager.AccessFailedAsync(user);
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Email or password incorrect"));
            }

            var roles = await _userManager.GetRolesAsync(user);
            return await Task.FromResult(new UserLoginResponse
            {
                JwtToken = _jwt.GenerateJwt(user, roles)
            });
        }

        public override async Task<Empty> CreateRole(CreateRoleRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Role name should be provided"));
            }

            var newRole = new Role { Name = request.RoleName };

            var roleResult = await _roleManager.CreateAsync(newRole);

            if (roleResult.Succeeded)
            {
                return await Task.FromResult(new Empty());
            }

            throw new RpcException(new Status(StatusCode.Internal, roleResult.Errors.First().ToString()));
        }

        public override async Task<Empty> AddUserToRole(AddUserToRoleRequest request, ServerCallContext context)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == request.Email);
            var result = await _userManager.AddToRoleAsync(user, request.RoleName);

            if (result.Succeeded)
            {
                return await Task.FromResult(new Empty());
            }

            throw new RpcException(new Status(StatusCode.Internal, result.Errors.First().ToString()));
        }
    }
}