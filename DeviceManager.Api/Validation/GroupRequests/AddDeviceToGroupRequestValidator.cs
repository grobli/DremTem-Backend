using DeviceManager.Core;
using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Validation.GroupRequests
{
    public class AddDeviceToGroupRequestValidator : AbstractValidator<AddDeviceToGroupRequest>
    {
        public AddDeviceToGroupRequestValidator()
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