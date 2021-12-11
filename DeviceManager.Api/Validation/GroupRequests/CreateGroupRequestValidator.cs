using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Validation.GroupRequests
{
    public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupRequestValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty();

            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();
        }
    }
}