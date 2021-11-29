using FluentValidation;
using Shared.Extensions;
using UserIdentity.Core.Proto;

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