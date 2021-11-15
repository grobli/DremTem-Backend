using System;
using Microsoft.AspNetCore.Identity;

namespace UserGrpcService.Models.Auth
{
    public class Role : IdentityRole<Guid>
    {
    }
}