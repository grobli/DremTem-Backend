using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace UserIdentity.Api.Validators
{
    public class GetUserByIdRequestValidator : AbstractValidator<GetUserByIdRequest>
    {
        public GetUserByIdRequestValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .Guid();
        }
    }
}