using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Shared.Extensions;
using Shared.Proto;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator(IPasswordValidator<User> passwordValidator, UserManager<User> userManager)
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.OldPassword)
                .NotEmpty();
            
            RuleFor(r => r.NewPassword)
                .NotEmpty()
                .NotEqual(r => r.OldPassword)
                .WithMessage("New password and old password must be different");
        }
    }
}