using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UserIdentity.Core.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        [Required] public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}