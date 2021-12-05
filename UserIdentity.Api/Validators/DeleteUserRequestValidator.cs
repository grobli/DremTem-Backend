using FluentValidation;
using Shared.Extensions;
using Shared.Proto.User;

namespace UserIdentity.Api.Validators
{
    public class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserRequestValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .Guid();
        }
    }
}