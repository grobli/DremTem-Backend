using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UserGrpcService.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        [Required] public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}