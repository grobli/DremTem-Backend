using FluentValidation;
using Shared.Proto;
using UserIdentity.Data.Configurations;

namespace UserIdentity.Api.Validators
{
    public class UserSignUpRequestValidator : AbstractValidator<UserSignUpRequest>
    {
        public UserSignUpRequestValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();
            
            RuleFor(u => u.FirstName)
                .MaximumLength(UserConfiguration.FirstNameMaxLength);
            
            RuleFor(u => u.LastName)
                .MaximumLength(UserConfiguration.LastNameMaxLength);
        }
    }
}