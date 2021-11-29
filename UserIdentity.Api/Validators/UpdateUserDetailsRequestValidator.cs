using FluentValidation;
using Shared.Extensions;
using UserIdentity.Core.Proto;
using UserIdentity.Data.Configurations;

namespace UserIdentity.Api.Validators
{
    public class UpdateUserDetailsRequestValidator : AbstractValidator<UpdateUserDetailsRequest>
    {
        public UpdateUserDetailsRequestValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.FirstName)
                .MaximumLength(UserConfiguration.FirstNameMaxLength);

            RuleFor(r => r.LastName)
                .MaximumLength(UserConfiguration.LastNameMaxLength);
        }
    }
}