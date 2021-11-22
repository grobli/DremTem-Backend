using FluentValidation;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Validators
{
    public class GetUserByIdRequestValidator : AbstractValidator<GetUserByIdRequest>
    {
        public GetUserByIdRequestValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .MustBeValidGuid();
        }
    }
}