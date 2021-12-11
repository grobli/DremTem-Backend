using FluentValidation;
using Shared.Proto;

namespace UserIdentity.Api.Validators
{
    public class GetUserByEmailValidator : AbstractValidator<GetUserByEmailRequest>
    {
        public GetUserByEmailValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}