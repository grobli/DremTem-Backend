using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Extensions
{
    public static class UserManagerExtension
    {
        public static async Task<UserDto> CollectUserDataAsync(this UserManager<User> self, [NotNull] User user,
            IMapper mapper)
        {
            var userInfo = mapper.Map<User, UserDto>(user);
            var roles = await self.GetRolesAsync(user);
            userInfo.Roles.Add(roles);
            return userInfo;
        }
    }
}