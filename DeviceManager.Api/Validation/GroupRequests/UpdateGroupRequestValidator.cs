using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

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