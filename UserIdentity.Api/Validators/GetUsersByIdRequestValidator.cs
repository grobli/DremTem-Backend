using FluentValidation;
using Shared.Extensions;
using Shared.Proto.User;

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