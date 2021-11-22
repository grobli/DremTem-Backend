using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Services.Rpc
{
    public class UserService : UserInfoGrpc.UserInfoGrpcBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        // validators
        private readonly IValidator<GetUserByIdRequest> _getUserByIdValidator;
        private readonly IValidator<GetUserByEmailRequest> _getUserByEmailValidator;
        private readonly IValidator<UpdateUserDetailsRequest> _updateUserDetailsValidator;

        public UserService(
            UserManager<User> userManager,
            ILogger<UserService> logger,
            IMapper mapper,
            IValidator<GetUserByIdRequest> getUserByIdValidator,
            IValidator<GetUserByEmailRequest> getUserByEmailValidator,
            IValidator<UpdateUserDetailsRequest> updateUserDetailsValidator)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;

            _getUserByIdValidator = getUserByIdValidator;
            _getUserByEmailValidator = getUserByEmailValidator;
            _updateUserDetailsValidator = updateUserDetailsValidator;
        }

        public override async Task GetAllUsers(GetAllUsersRequest request,
            IServerStreamWriter<UserResource> responseStream,
            ServerCallContext context)
        {
            foreach (var user in _userManager.Users)
            {
                await responseStream.WriteAsync(await CollectUserDataAsync(user));
            }
        }

        public override async Task<UserResource> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var validationResult = await _getUserByIdValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(request.Id);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return await CollectUserDataAsync(user);
        }

        public override async Task<UserResource> GetUserByEmail(GetUserByEmailRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getUserByEmailValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == request.Email);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return await CollectUserDataAsync(user);
        }

        public override async Task<UserResource> UpdateUserDetails(UpdateUserDetailsRequest request,
            ServerCallContext context)
        {
            var validationResult = await _updateUserDetailsValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(request.Id);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                return await Task.FromResult(await CollectUserDataAsync(user));
            }

            throw new RpcException(new Status(StatusCode.Internal, updateResult.Errors.First().ToString()));
        }

        public override Task<Empty> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            // TODO: Off there is so much to do when deleting users omg 
            return base.DeleteUser(request, context);
        }

        private async Task<UserResource> CollectUserDataAsync([NotNull] User user)
        {
            var userInfo = _mapper.Map<User, UserResource>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userInfo.Roles.Add(roles);

            return userInfo;
        }
    }
}