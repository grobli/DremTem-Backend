using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserGrpcService.Data;
using UserGrpcService.Proto;

namespace UserGrpcService.Services
{
    // public class UserService : User.UserBase
    // {
    //     private readonly ILogger<UserService> _logger;
    //     private readonly UserDbContext _dbContext;
    //     private readonly IMapper _mapper;
    //
    //     private readonly EmailAddressAttribute _emailAddressAttribute;
    //
    //     public UserService(ILogger<UserService> logger, UserDbContext dbContext, IMapper mapper)
    //     {
    //         _logger = logger;
    //         _dbContext = dbContext;
    //         _mapper = mapper;
    //         _emailAddressAttribute = new EmailAddressAttribute();
    //     }
    //
    //     [return: NotNull]
    //     private async Task<Models.User> GetUserFromDbAsync(int id)
    //     {
    //         var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    //         if (user == null)
    //         {
    //             throw new RpcException(new Status(StatusCode.NotFound,
    //                 $"Specified user with Id=\"{id}\" does not exist"));
    //         }
    //
    //         return user;
    //     }
    //
    //     private bool IsValidEmail(string email) =>
    //         !string.IsNullOrWhiteSpace(email) && _emailAddressAttribute.IsValid(email);
    //
    //
    //     public override async Task<UserGrpcModel> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    //     {
    //         var user = await GetUserFromDbAsync(request.Id);
    //
    //         return await Task.FromResult(_mapper.Map<UserGrpcModel>(user));
    //     }
    //
    //     public override async Task<UserGrpcModel> GetUserByEmail(GetUserByEmailRequest request,
    //         ServerCallContext context)
    //     {
    //         var email = request.Email.Trim();
    //         if (!IsValidEmail(email))
    //         {
    //             throw new RpcException(new Status(StatusCode.InvalidArgument,
    //                 $"Provided Email=\"{email}\" is not valid"));
    //         }
    //
    //         var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    //         if (user == null)
    //         {
    //             throw new RpcException(new Status(StatusCode.NotFound,
    //                 $"Specified user with Email=\"{email}\" does not exist"));
    //         }
    //
    //         return await Task.FromResult(_mapper.Map<UserGrpcModel>(user));
    //     }
    //
    //     public override async Task<UserGrpcModel> CreateUser(CreateUserRequest request, ServerCallContext context)
    //     {
    //         var email = request.Email.Trim();
    //         if (!IsValidEmail(email))
    //         {
    //             throw new RpcException(new Status(StatusCode.InvalidArgument,
    //                 $"Provided Email=\"{email}\" is not valid"));
    //         }
    //
    //         if (string.IsNullOrWhiteSpace(request.FirstName))
    //         {
    //             throw new RpcException(new Status(StatusCode.InvalidArgument,
    //                 $"FirstName can not be null, empty or whitespace only"));
    //         }
    //
    //         if (await _dbContext.Users.AnyAsync(u => u.Email == email))
    //         {
    //             throw new RpcException(new Status(StatusCode.InvalidArgument,
    //                 $"User with provided Email=\"{email}\" already exists in the database"));
    //         }
    //
    //         var user = Models.User.CreateUser(request.FirstName, email, request.Password,
    //             GetLastNameValue());
    //
    //         try
    //         {
    //             await _dbContext.Users.AddAsync(user);
    //             await _dbContext.SaveChangesAsync();
    //         }
    //         catch (DbUpdateException e)
    //         {
    //             _logger.LogError(e, "Unable to save changes");
    //             throw new RpcException(new Status(StatusCode.Internal,
    //                 "Unable to save changes." +
    //                 "Try again, and if the problem persists see your system administrator."));
    //         }
    //
    //         return await Task.FromResult(_mapper.Map<UserGrpcModel>(user));
    //
    //         string GetLastNameValue() =>
    //             request.LastNameCase switch
    //             {
    //                 CreateUserRequest.LastNameOneofCase.None => null,
    //                 CreateUserRequest.LastNameOneofCase.LastNameValue => request.LastNameValue,
    //                 _ => throw new ArgumentException("Invalid message - LastNameCase")
    //             };
    //     }
    //
    //
    //     public override async Task<Empty> UpdateUserPassword(UpdatePasswordRequest request, ServerCallContext context)
    //     {
    //         var user = await GetUserFromDbAsync(request.Id);
    //         user.ChangePassword(request.Password);
    //
    //         try
    //         {
    //             _dbContext.Users.Update(user);
    //             await _dbContext.SaveChangesAsync();
    //         }
    //         catch (DbUpdateException e)
    //         {
    //             _logger.LogError(e, "Unable to save changes");
    //             throw new RpcException(new Status(StatusCode.Internal,
    //                 "Unable to save changes." +
    //                 "Try again, and if the problem persists see your system administrator."));
    //         }
    //
    //         return await Task.FromResult(new Empty());
    //     }
    //
    //     public override async Task<UserGrpcModel> UpdateUserName(UpdateNameRequest request, ServerCallContext context)
    //     {
    //         var user = await GetUserFromDbAsync(request.Id);
    //
    //         var firstName = GetFirstNameValue();
    //         user.FirstName = string.IsNullOrWhiteSpace(firstName) ? user.FirstName : firstName;
    //
    //         var lastName = GetLastNameValue();
    //         user.LastName = string.IsNullOrWhiteSpace(lastName) ? user.LastName : lastName;
    //
    //         try
    //         {
    //             _dbContext.Users.Update(user);
    //             await _dbContext.SaveChangesAsync();
    //         }
    //         catch (DbUpdateException e)
    //         {
    //             _logger.LogError(e, "Unable to save changes");
    //             throw new RpcException(new Status(StatusCode.Internal,
    //                 "Unable to save changes." +
    //                 "Try again, and if the problem persists see your system administrator."));
    //         }
    //
    //         return await Task.FromResult(_mapper.Map<UserGrpcModel>(user));
    //
    //         string GetFirstNameValue() =>
    //             request.FirstNameCase switch
    //             {
    //                 UpdateNameRequest.FirstNameOneofCase.None => null,
    //                 UpdateNameRequest.FirstNameOneofCase.FirstNameValue => request.FirstNameValue,
    //                 _ => throw new ArgumentException("Invalid message - FirstNameCase")
    //             };
    //
    //         string GetLastNameValue() =>
    //             request.LastNameCase switch
    //             {
    //                 UpdateNameRequest.LastNameOneofCase.None => null,
    //                 UpdateNameRequest.LastNameOneofCase.LastNameValue => request.LastNameValue,
    //                 _ => throw new ArgumentException("Invalid message - LastNameCase")
    //             };
    //     }
    //
    //     public override async Task<Empty> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    //     {
    //         var user = await GetUserFromDbAsync(request.Id);
    //
    //         // TODO: Notify all microservices that user has been deleted 
    //
    //         try
    //         {
    //             _dbContext.Users.Remove(user);
    //             await _dbContext.SaveChangesAsync();
    //         }
    //         catch (DbUpdateException e)
    //         {
    //             _logger.LogError(e, "Unable to save changes");
    //             throw new RpcException(new Status(StatusCode.Internal,
    //                 "Unable to save changes." +
    //                 "Try again, and if the problem persists see your system administrator."));
    //         }
    //
    //         return await Task.FromResult(new Empty());
    //     }
    // }
}