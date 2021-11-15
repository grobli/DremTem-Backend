using FluentValidation;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Validators
{
    public class UserSignUpRequestValidator : AbstractValidator<UserSignUpRequest>
    {
        public UserSignUpRequestValidator()
        {
            RuleFor(u => u.Email).EmailAddress();
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(u => u.LastName)
                .MaximumLength(100);
        }
    }
}