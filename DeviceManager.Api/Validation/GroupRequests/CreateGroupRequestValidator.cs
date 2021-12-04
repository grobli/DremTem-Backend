using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

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