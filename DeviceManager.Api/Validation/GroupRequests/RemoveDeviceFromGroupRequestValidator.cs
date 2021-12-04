using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validation.GroupRequests
{
    public class RemoveDeviceFromGroupRequestValidator : AbstractValidator<RemoveDeviceFromGroupRequest>
    {
        public RemoveDeviceFromGroupRequestValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.DeviceId)
                .NotEmpty();

            RuleFor(r => r.GroupId)
                .NotEmpty();
        }
    }
}