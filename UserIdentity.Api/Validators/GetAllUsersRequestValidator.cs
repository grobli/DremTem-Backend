using FluentValidation;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Validators
{
    public class GetAllUsersRequestValidator : AbstractValidator<GetAllUsersRequest>
    {
        public GetAllUsersRequestValidator()
        {
            RuleFor(r => r.PageSize)
                .GreaterThan(0);

            RuleFor(r => r.PageNumber)
                .GreaterThanOrEqualTo(1);
        }
    }
}