using FluentValidation;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Validators
{
    public class GetUserByEmailValidator : AbstractValidator<GetUserByEmailRequest>
    {
        public GetUserByEmailValidator()
        {
            RuleFor(r => r.Email).EmailAddress();
        }
    }
}