using FluentValidation;
using Shared.Extensions;
using Shared.Proto.Group;

namespace DeviceManager.Api.Validation.GroupRequests
{
    public class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
    {
        public UpdateGroupRequestValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();
        }
    }
}